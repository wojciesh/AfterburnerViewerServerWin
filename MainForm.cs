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

        private void SetAbConfig(IAfterburnerConfig? afterburnerConfig)
        {
            abConfigProvider = afterburnerConfig;

            settings.abConfigFile = abConfigProvider == null
                ? String.Empty
                : abConfigProvider.ConfigFile;

            txtDir.Text = Path.GetDirectoryName(settings.abConfigFile);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = APPLICATION_TITLE;
            InitIpc();
            RestartIpc();

            if (abConfigProvider == null)
            {
                if (String.IsNullOrEmpty(settings.abConfigFile))
                {
                    LogMe("-----------------------------------");
                    LogMe("No Afterburner Dir selected");
                    LogMe("Please select the directory where MSI Afterburner is installed");
                    LogMe("-----------------------------------");
                }
                else
                    LogMe($"Can't load Afterburner config from {settings.abConfigFile}");
            }
            else
                LogMe($"Loaded Afterburner config from {abConfigProvider.ConfigFile}");

            SetSourceFromAbConfig();

            RestartMeasurements(GetSource());
        }

        private void SetSourceFromAbConfig()
        {
            if (abConfigProvider?.IsConfigFileValid() ?? false)
            {
                SetSource(abConfigProvider.GetHistoryLogPath());
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            DestroyIpc();
            DestroyMeasurements();
        }

        protected AfterburnerMeasurementsProvider CreateMeasurementsProvider()
        {
            var mp = new AfterburnerMeasurementsProvider();

            mp.OnError += (s, msg) => LogMe($"Error: {msg}");

            mp.OnNewMeasurements += HandleNewMeasurements;

            return mp;

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

        protected void UpdateMeasurementPreview(string measurement)
        {
            this.BeginInvoke(() =>
            {
                try { txtMeasurementsPreview.Text = measurement; }
                catch { }
            });
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
            if (String.IsNullOrEmpty(msg)) return;

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

            if (String.IsNullOrEmpty(newLogs)) return;

            log.Text += newLogs;

            log.SelectionStart = log.Text.Length;
            log.ScrollToCaret();
        }


        private void btSelectSourceFile_Click(object sender, EventArgs e)
        {
            RestartMeasurements(GetSourceFileFromUser());
        }


        protected string GetSource()
        {
            return settings.source;
        }

        protected bool SetSource(string? sourceFile)
        {
            if (!measurementsProvider.IsValidSource(sourceFile))
                return false;
            Debug.Assert(sourceFile != null);
            settings.source = sourceFile;
            return true;
        }

        protected string? GetSourceFileFromUser()
        {
            string filePath = txtFile.Text;

            if (!String.IsNullOrWhiteSpace(filePath))
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

        private void btOpenDir_Click(object sender, EventArgs e)
        {
            SetAbConfig(CreateAbConfig(Path.Combine(
                GetAbConfigDirFromUser() ?? String.Empty,
                "Profiles",
                "MSIAfterburner.cfg")));

            SetSourceFromAbConfig();
        }

        private IAfterburnerConfig? CreateAbConfig(string? abConfigFile)
        {
            if (String.IsNullOrEmpty(abConfigFile))
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

        protected string? GetAbConfigDirFromUser()
        {
            dlgDir.AutoUpgradeEnabled = true;

            string dirPath = txtDir.Text;
            if (!String.IsNullOrWhiteSpace(dirPath))
            {
                dlgDir.InitialDirectory = dirPath;
                dlgDir.SelectedPath = dirPath;
            }

            if (dlgDir.ShowDialog() != DialogResult.OK)
                return null;

            var fn = dlgDir.SelectedPath;

            txtDir.Text = fn;

            return fn;
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

        private void btSetABConfig_Click(object sender, EventArgs e)
        {
            DialogResult wantBackup = MessageBox.Show("This will modify your MSI Afterburner config file?\r\nDo you want to make a backup?", "Warning", MessageBoxButtons.YesNoCancel);
            if (wantBackup == DialogResult.Cancel)
                return;

            if (!(abConfigProvider?.IsConfigFileValid() ?? false))
            {
                LogMe("ERROR: No valid Afterburner config file found");
                return;
            }

            if (wantBackup == DialogResult.Yes
                && !backupABConfigFile())
            {
                LogMe("STOP: Not modifying Afterburner config file without backup");
                return;
            }

            SetHistoryLogOn();

        }

        private void SetHistoryLogOn()
        {
            throw new NotImplementedException();
        }

        private bool backupABConfigFile()
        {
            try
            {
                string backupFile = Path.Combine(
                                    Path.GetDirectoryName(abConfigProvider.ConfigFile) ?? String.Empty,
                                    $"MSIAfterburner.cfg.{DateTime.Now:yyyyMMddHHmmss}.bak");
                File.Copy(abConfigProvider?.ConfigFile ?? String.Empty, backupFile, true);

                LogMe($"Backup of Afterburner config file created: {backupFile}");
                return true;
            }
            catch (Exception ex)
            {
                LogMe($"ERROR: Can't backup Afterburner config file: {ex.Message}");
                return false;
            }
        }
    }
}
