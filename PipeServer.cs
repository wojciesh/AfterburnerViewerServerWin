using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AfterburnerViewerServerWin
{
    public class PipeServer : IDisposable
    {
        bool running;
        Thread? runningThread;
        EventWaitHandle terminateHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private bool disposedValue;

        public string PipeName { get; }

        public event EventHandler? OnNewClient;
        public event EventHandler? OnClientDisconnected;
        public event EventHandler<string>? OnMessageSend;

        public PipeServer(string pipeName)
        {
            PipeName = pipeName;
        }

        void ServerLoop()
        {
            while (running)
            {
                ProcessNextClient();
            }

            terminateHandle.Set();
        }

        public void Run()
        {
            running = true;
            runningThread = new Thread(ServerLoop);
            runningThread.Start();
        }

        public void Stop()
        {
            bool breakWaiting = running;

            running = false;

            if (breakWaiting)
            {
                Thread.Sleep(200);

                //to break WaitForConnection
                using (NamedPipeClientStream npcs = new NamedPipeClientStream(PipeName))
                {
                    npcs.Connect(100);
                }
            }

            terminateHandle.WaitOne(1000);
        }

        public virtual string ProcessRequest(string message)
        {
            return "";
        }

        public void ProcessClientThread(object o)
        {
            NamedPipeServerStream pipeStream = (NamedPipeServerStream)o;
            try
            {
                OnNewClient?.Invoke(this, EventArgs.Empty);

                string msg;

                while (running && pipeStream.IsConnected)
                {
                    Thread.Sleep(300);
                    lock (_lock_msg)
                    {
                        msg = message;
                        message = null;
                    }
                    if (String.IsNullOrEmpty(msg)) continue;

                    byte[] messageBytes = Encoding.UTF8.GetBytes(msg);
                    pipeStream.Write(messageBytes, 0, messageBytes.Length);
                    pipeStream.Flush();
                    pipeStream.WaitForPipeDrain();

                    OnMessageSend?.Invoke(this, msg);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                OnClientDisconnected?.Invoke(this, EventArgs.Empty);

                if (pipeStream.IsConnected)
                    pipeStream.Disconnect();

                pipeStream.Close();
                pipeStream.Dispose();
            }
        }

        public void ProcessNextClient()
        {
            try
            {
                NamedPipeServerStream pipeStream = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 254);
                pipeStream.WaitForConnection();

                //Spawn a new thread for each request and continue waiting
                Thread t = new Thread(ProcessClientThread);
                t.Start(pipeStream);
            }
            catch (Exception e)
            {
                //If there are no more avail connections (254 is in use already) then just keep looping until one is avail
            }
        }

        private readonly object _lock_msg = new object ();
        private string? message = null;

        public void WriteToAllClients(string msg)
        {
            if (String.IsNullOrEmpty(msg)) return;

            lock(_lock_msg)
            {
                message = msg;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                    runningThread?.Join();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
