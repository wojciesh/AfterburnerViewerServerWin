using System.Diagnostics;
using System.Text;

namespace AfterburnerViewerServerWin
{
    public partial class MainForm : Form
    {
        private const string PIPE_NAME = "ab2sd-1";

        private readonly IpcServer ipcServer;
        private readonly AfterburnerMeasurementsProvider abProvider;
        private readonly StringBuilder logBuffer = new();
        private readonly object lock_logBuffer = new();


        public MainForm()
        {
            InitializeComponent();

            ipcServer = new IpcServer(PIPE_NAME);

            abProvider = new AfterburnerMeasurementsProvider();
            abProvider.OnMeasurement += (s, measurement) =>
            {
                LogMe($"Measurement: {measurement}");
                ipcServer.Write(measurement);
            };
            abProvider.OnError += (s, msg) => LogMe($"Error: {msg}");
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            InitIpc();
            RestartIpc();
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


        private bool StartMeasurements(string srcFile)
        {
            abProvider.Stop(false);
            return abProvider.Start(srcFile);
        }

        private void DestroyMeasurements()
        {
            abProvider.Dispose();
        }


        private void InitIpc()
        {
            ipcServer.OnMessageSend += (s, msg) => LogMe($"Send: {msg}");
            ipcServer.OnError += (s, msg) => LogMe($"Error: {msg}");
            ipcServer.OnNewClient += (s, e) => LogMe("New client connected");
            ipcServer.OnClientDisconnected += (s, e) => LogMe("Client disconnected");
            ipcServer.OnServerStopped += (s, e) => LogMe("Server stopped");
            ipcServer.OnServerStarted += (s, e) => LogMe("Server started");
        }

        private void DestroyIpc()
        {
            ipcServer.Dispose();
        }

        private void RestartIpc()
        {
            LogMe("Restarting IPC server...");
            ipcServer.RestartServer();
        }


        private void LogMe(string msg)
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


        private void btSelectABFile_Click(object sender, EventArgs e)
        {
            SelectSource();
        }

        private void SelectSource()
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
                return;

            var fn = dlgOpen.FileName;

            if (!StartMeasurements(fn))
            {
                LogMe("Failed to start measurements for source: " + fn);
                return;
            }

            txtFile.Text = fn;
        }
    }
}
