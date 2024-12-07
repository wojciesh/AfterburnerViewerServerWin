using System.Diagnostics;
using System.Text;

namespace AfterburnerViewerServerWin
{
    public partial class MainForm : Form
    {
        private const string PIPE_NAME = "ab2sd-1";

        private readonly IpcServer ipcServer;
        private readonly IMeasurementsProvider abProvider;
        private readonly StringBuilder logBuffer = new();
        private readonly object lock_logBuffer = new();


        public MainForm()
        {
            InitializeComponent();

            ipcServer = new IpcServer(PIPE_NAME);

            abProvider = new AfterburnerMeasurementsProvider();
            abProvider.OnMeasurement += (s, measurement) =>
            {
                logMe($"Measurement: {measurement}");
            };
            abProvider.OnError += (s, msg) => logMe($"Error: {msg}");
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            initIpc();
            restartIpc();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            destroyIpc();
            destroyMeasurements();
        }


        private void sendMeasurementTimer_Tick(object sender, EventArgs e)
        {
            //try
            //{
            //    string nowUTC = DateTime.UtcNow.ToString("HH:mm:ss");
            //    ipcServer.Write(nowUTC);
            //}
            //catch (Exception ex)
            //{
            //    logMe($"Error in timer: {ex.Message}");
            //    destroyIpc();
            //    throw;
            //}
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            restartIpc();
        }


        private bool startMeasurements(string srcFile)
        {
            abProvider.Stop(false);
            return abProvider.Start(srcFile);
        }

        private void destroyMeasurements()
        {
            abProvider.Dispose();
        }


        private void initIpc()
        {
            ipcServer.OnMessageSend += (s, msg) => logMe($"Send: {msg}");
            ipcServer.OnError += (s, msg) => logMe($"Error: {msg}");
            ipcServer.OnNewClient += (s, e) => logMe("New client connected");
            ipcServer.OnClientDisconnected += (s, e) => logMe("Client disconnected");
            ipcServer.OnServerStopped += (s, e) => logMe("Server stopped");
            ipcServer.OnServerStarted += (s, e) => logMe("Server started");
        }

        private void destroyIpc()
        {
            ipcServer.Dispose();
        }

        private void restartIpc()
        {
            logMe("Restarting IPC server...");
            ipcServer.RestartServer();
        }


        private void logMe(string msg)
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
            
            if (!startMeasurements(fn))
            {
                logMe("Failed to start measurements for source: " + fn);
                return;
            }

            txtFile.Text = fn;
        }
    }
}
