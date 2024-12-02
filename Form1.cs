using System.IO.Pipes;
using System.Text;

namespace AfterburnerViewerServerWin
{
    public partial class Form1 : Form
    {
        private readonly IpcServer ipcServer;

        public Form1()
        {
            InitializeComponent();

            ipcServer = new IpcServer("ab2sd4");
            ipcServer.OnError += (s, msg) => this.BeginInvoke(new Action(() => log.Text += $"Error: {msg}\n"));
            ipcServer.OnDisconnected += (s, e) => this.BeginInvoke(new Action(() => log.Text += "Disconnected!\n"));
            ipcServer.OnNewClient += (s, user) => this.BeginInvoke(new Action(() => log.Text += $"New client: {user}\n"));
            ipcServer.OnClientDisconnected += (s, user) => this.BeginInvoke(new Action(() => log.Text += $"Client disconnected: {user}\n"));
            ipcServer.OnMessageSend += (s, msg) => this.BeginInvoke(new Action(() => log.Text += $"Send: {msg}\n"));
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ipcServer.Dispose();
        }

        private void ipcTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                string nowUTC = DateTime.UtcNow.ToString("HH:mm:ss");
                ipcServer.Write(nowUTC);

                //log.Text += $"Sent: {nowUTC}\n";
            }
            catch (Exception ex)
            {
                log.Text += $"Error: {ex.Message}\n";
                ipcServer.Dispose();
                throw;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            log.Text += "Connecting...\n";
            ipcServer.Start();
        }
    }
}
