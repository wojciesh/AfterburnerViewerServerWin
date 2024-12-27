using AfterburnerViewerServerWin.abconfig;
using Config.Net;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;

namespace AfterburnerViewerServerWin
{
    [SupportedOSPlatform("windows6.1")]
    public partial class MainForm : Form
    {
        private const string APPLICATION_TITLE = "AfterburnerToStreamDeck-Server v1.0";
        private const string PIPE_NAME = "ab2sd-1";

        private readonly IpcServer ipcServer;
        private readonly IMeasurementsProvider measurementsProvider;
        private IAfterburnerConfig? abConfigProvider;
        private readonly StringBuilder logBuffer = new();
        private readonly object lock_logBuffer = new();

        private readonly IAppConfig settings;


        public MainForm()
        {
            InitializeComponent();

            settings = new ConfigurationBuilder<IAppConfig>()
                .UseJsonFile("config.json")
                .Build();

            SetAbConfig(CreateAbConfig(settings.abConfigFile));

            ipcServer = new IpcServer(PIPE_NAME);
            measurementsProvider = CreateMeasurementsProvider();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = APPLICATION_TITLE;
            
            printAbConfigLoadResult();

            if (abConfigProvider != null)
                loadSourceFile();

            InitIpc();
            RestartIpc();
            RestartMeasurements(GetSource());


            void loadSourceFile()
            {
                if (string.IsNullOrEmpty(settings.sourceFile))
                    SetSourceFromAbConfig();
                else
                    SetSource(settings.sourceFile);

                UpdateSourceGUI();
            }

            void printAbConfigLoadResult()
            {
                if (abConfigProvider != null)
                    LogMe($"Loaded Afterburner config from {abConfigProvider.ConfigFile}");
                else
                {
                    if (string.IsNullOrEmpty(settings.abConfigFile))
                    {
                        LogMe("-----------------------------------");
                        LogMe("No Afterburner dir selected");
                        LogMe("Please select the directory where MSI Afterburner is installed");
                        LogMe("-----------------------------------");
                    }
                    else
                        LogMe($"WARNING: Can't load Afterburner config from {settings.abConfigFile}");
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            DestroyIpc();
            DestroyMeasurements();
        }


        private IAfterburnerConfig? CreateAbConfig(string? abConfigFile)
        {
            if (string.IsNullOrEmpty(abConfigFile))
                return null;

            try
            {
                return new AfterburnerConfig(abConfigFile);
            }
            catch (Exception ex)
            {
                LogMe(ex.Message);
                return null;
            }
        }

        protected void SetAbConfig(IAfterburnerConfig? afterburnerConfig)
        {
            abConfigProvider = afterburnerConfig;

            settings.abConfigFile = abConfigProvider == null
                ? string.Empty
                : abConfigProvider.ConfigFile;

            txtDir.Text = Path.GetDirectoryName(settings.abConfigFile);
        }

        protected string? GetAbConfigDirFromUser()
        {
            dlgDir.AutoUpgradeEnabled = true;

            if (dlgDir.ShowDialog() != DialogResult.OK)
                return null;

            var fn = dlgDir.SelectedPath;

            txtDir.Text = fn;

            return fn;
        }

        
        protected string GetSource()
        {
            return settings.sourceFile;
        }

        protected bool SetSource(string? sourceFile)
        {
            if (!measurementsProvider.IsValidSource(sourceFile))
                return false;
            Debug.Assert(sourceFile != null);
            settings.sourceFile = sourceFile;
            return true;
        }

        protected string? GetSourceFileFromUser()
        {
            string filePath = txtFile.Text;

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                dlgOpen.InitialDirectory = Path.GetDirectoryName(filePath);
                dlgOpen.FileName = Path.GetFileName(filePath);
            }
            else
            {
                dlgOpen.FileName = "HardwareMonitoring.hml";
            }

            if (dlgOpen.ShowDialog() != DialogResult.OK)
                return null;

            var fn = dlgOpen.FileName;

            txtFile.Text = fn;

            return fn;
        }

        protected void SetSourceFromAbConfig()
        {
            if (abConfigProvider?.IsConfigFileValid() ?? false)
            {
                SetSource(abConfigProvider.GetHistoryLogPath());
            }
        }

        private void UpdateSourceGUI()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(() =>
                {
                    txtFile.Text = GetSource();
                });
            }
            else
            {
                txtFile.Text = GetSource();
            }
        }


        protected IMeasurementsProvider CreateMeasurementsProvider()
        {
            var mp = new AfterburnerMeasurementsProvider();

            mp.OnError += (s, msg) => LogMe($"Error: {msg}");

            mp.OnNewMeasurements += HandleNewMeasurements;

            return mp;

        }

        protected void RestartMeasurements(string? sourceFile)
        {
            if (SetSource(sourceFile))
            {
                txtFile.Text = GetSource();
                StartMeasurements();
            }
            else
            {
                txtFile.Clear();

                if (abConfigProvider != null)
                {
                    LogMe("-----------------------------------------");
                    LogMe("!!! No Afterburner History File found !!!");
                    LogMe("Please follow these steps:");
                    LogMe("  1. In MSI Afterburner go to Setting -> Monitoring and enable Logging History to file");
                    LogMe("  2. Select your History Log file in the menu above");
                    LogMe("-----------------------------------------");
                }
            }
        }

        protected bool StartMeasurements()
        {
            measurementsProvider.Stop(false);
            return measurementsProvider.Start(GetSource());
        }

        protected void DestroyMeasurements()
        {
            measurementsProvider.Dispose();
        }

        protected void HandleNewMeasurements(object? sender, List<AfterburnerMeasurement> measurements)
        {
            if (measurements == null || measurements.Count == 0)
                return;

            string measurementsJson = JsonSerializer.Serialize(measurements);

            ipcServer.Write(measurementsJson);

            UpdateMeasurementPreview(measurements
                .Select(m => $"{m.Type.Name}: {getFormattedValue(m)}{m.Type.Unit}")
                .Aggregate((a, b) => $"{a} | {b}")
            );


            string getFormattedValue(AfterburnerMeasurement m)
            {
                return m.Type.Format switch
                {
                    "%.3f" => m.Value.ToString("F3"),
                    "%.2f" => m.Value.ToString("F2"),
                    _ => m.Value.ToString("F1"),
                };
            }
        }

        protected void UpdateMeasurementPreview(string measurement)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(() =>
                {
                    try { txtMeasurementsPreview.Text = measurement; }
                    catch { }
                });
            }
            else
            {
                try { txtMeasurementsPreview.Text = measurement; }
                catch { }
            }
        }


        protected void InitIpc()
        {
            //ipcServer.OnMessageSend += (s, msg) => LogMe($"Send: {msg}");
            ipcServer.OnError += (s, msg) => LogMe($"Error: {msg}");
            ipcServer.OnNewClient += (s, e) => LogMe("New client connected");
            ipcServer.OnClientDisconnected += (s, e) => LogMe("Client disconnected");
            ipcServer.OnServerStopped += (s, e) => LogMe("Server stopped");
            ipcServer.OnServerStarted += (s, e) => LogMe("Server started");
        }

        protected void DestroyIpc()
        {
            ipcServer.Dispose();
        }

        protected void RestartIpc()
        {
            LogMe("Restarting IPC server...");
            ipcServer.RestartServer();
        }


        protected void LogMe(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;

            msg = $"{DateTime.Now:HH:mm:ss} {msg}";

            Debug.WriteLine(msg);

            lock (lock_logBuffer)
            {
                logBuffer.AppendLine(msg);
            }
        }

        private void logTimer_Tick(object sender, EventArgs e)
        {
            string newLogs;
            lock (lock_logBuffer)
            {
                newLogs = logBuffer.ToString();
                logBuffer.Clear();
            }

            if (string.IsNullOrEmpty(newLogs)) return;

            log.Text += newLogs;

            log.SelectionStart = log.Text.Length;
            log.ScrollToCaret();
        }


        private void btOpenDir_Click(object sender, EventArgs e)
        {
            string? dir = GetAbConfigDirFromUser();
            
            if (string.IsNullOrEmpty(dir)) 
                return;

            SetAbConfig(CreateAbConfig(Path.Combine(
                dir,
                "Profiles",
                "MSIAfterburner.cfg")));

            SetSourceFromAbConfig();
            UpdateSourceGUI();
            RestartMeasurements(GetSource());
        }

        private void btSelectSourceFile_Click(object sender, EventArgs e)
        {
            RestartMeasurements(GetSourceFileFromUser());
        }

        private void btRestartIpc_Click(object sender, EventArgs e)
        {
            RestartIpc();
        }

        private void btCopyMeasurement_Click(object sender, EventArgs e)
        {
            txtMeasurementsPreview.SelectAll();
            txtMeasurementsPreview.Copy();
            txtMeasurementsPreview.DeselectAll();
        }

        private void timerABSettings_Tick(object sender, EventArgs e)
        {
            txtABStatus.Text = $"History Log: {toOnOffUn(abConfigProvider?.IsHistoryLogEnabled())}"
                + $" | Recreate: {toOnOffUn(abConfigProvider?.IsRecreateHistoryLog())}"
                + $" | Limit: {abConfigProvider?.GetHistoryLogLimit() ?? -1}";

            static string toOnOffUn(bool? isOn)
            {
                return isOn == null
                        ? "N/A"
                        : isOn == true
                            ? "ON"
                            : "OFF";
            }
        }
    }
}
