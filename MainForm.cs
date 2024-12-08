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
        private readonly AfterburnerMeasurementsProvider measurementsProvider;
        private readonly StringBuilder logBuffer = new();
        private readonly object lock_logBuffer = new();

        private readonly IAppConfig settings;


        public MainForm()
        {
            InitializeComponent();

            settings = new ConfigurationBuilder<IAppConfig>()
                .UseJsonFile("config.json")
                .Build();

            ipcServer = new IpcServer(PIPE_NAME);
            measurementsProvider = CreateMeasurementsProvider();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = APPLICATION_TITLE;
            InitIpc();
            RestartIpc();
            RestartMeasurements(GetSource());
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            DestroyIpc();
            DestroyMeasurements();
        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            RestartIpc();
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
            else txtFile.Clear();
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
    }
}
