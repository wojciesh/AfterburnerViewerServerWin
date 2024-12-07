using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Text;

namespace AfterburnerViewerServerWin
{
    public class PipeServer : IDisposable
    {
        public string PipeName { get; }

        public event EventHandler? OnServerStarted;
        public event EventHandler? OnServerStopped;
        public event EventHandler? OnNewClient;
        public event EventHandler? OnClientDisconnected;
        public event EventHandler<string>? OnMessageSend;

        private volatile bool isRunning;
        private volatile bool isWaitingForClients;

        private string? message = null;
        private readonly object _lock_msg = new();

        protected volatile Thread? runningThread;
        protected readonly ConcurrentBag<Thread> clientThreads = [];
        protected readonly EventWaitHandle terminateHandle = new(false, EventResetMode.AutoReset);
        protected bool disposedValue;


        public PipeServer(string pipeName)
        {
            if (string.IsNullOrWhiteSpace(pipeName))
                throw new ArgumentException($"'{nameof(pipeName)}' cannot be null or whitespace.", nameof(pipeName));

            PipeName = pipeName;
        }


        public void Start()
        {
            if (isRunning || runningThread != null) return;

            runningThread = new Thread(ServerLoop);
            runningThread.Start();
        }

        public void Stop()
        {
            isRunning = false;

            clearClientEvents();

            breakWaitForConnection();

            terminateHandle.WaitOne(1000);

            runningThread?.Join(1000);
            runningThread = null;

            foreach (var clientThread in clientThreads)
            {
                clientThread.Join(1000);
            }
            clientThreads.Clear();

            OnServerStopped?.Invoke(this, EventArgs.Empty);
        }


        private void breakWaitForConnection()
        {
            if (isWaitingForClients)
            {
                // break blocking WaitForConnection
                using NamedPipeClientStream dummy = new(PipeName);
                dummy.Connect(100);
            }
        }

        public void WriteToAllClients(string msg)
        {
            if (String.IsNullOrEmpty(msg)) return;

            lock (_lock_msg)
            {
                message = msg;
            }
        }

        protected void ServerLoop()
        {
            try
            {
                OnServerStarted?.Invoke(this, EventArgs.Empty);

                terminateHandle.Reset();
                isRunning = true;

                while (isRunning)
                {
                    var clientPipe = waitForNextClient();
                    if (clientPipe != null) 
                        handleClientOnNewThread(clientPipe);
                }

            } finally {
                isRunning = false;
                terminateHandle.Set();
            }
        }

        protected NamedPipeServerStream? waitForNextClient()
        {
            try
            {
                isWaitingForClients = true;

                NamedPipeServerStream pipeStream = new(PipeName, PipeDirection.InOut, 254);

                pipeStream.WaitForConnection(); // this blocks, we have to break it on dispose

                return pipeStream;
            }
            catch (Exception)
            {
                //If there are no more avail connections (254 is in use already) then just keep looping until one is avail
                return null;
            }
            finally
            {
                isWaitingForClients = false;
            }
        }

        protected void handleClientOnNewThread(NamedPipeServerStream pipeStream)
        {
            // Spawn a new thread for each request and continue waiting
            var t = new Thread(clientThread);
            t.Start(pipeStream);
            clientThreads.Add(t);
        }

        protected void clientThread(object? o)
        {
            var clientPipe = (NamedPipeServerStream)o!;
            OnNewClient?.Invoke(this, EventArgs.Empty);
            
            try
            {
                while (isRunning && clientPipe.IsConnected)
                {
                    Thread.Sleep(300);
                    
                    var msg = popMessageToSend();
                    if (String.IsNullOrEmpty(msg)) 
                        continue;

                    writeToClient(clientPipe, msg);

                    OnMessageSend?.Invoke(this, msg);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (clientPipe.IsConnected)
                    clientPipe.Disconnect();

                clientPipe.Close();
                clientPipe.Dispose();

                OnClientDisconnected?.Invoke(this, EventArgs.Empty);
            }

            static void writeToClient(NamedPipeServerStream pipeStream, string msg)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(msg);
                pipeStream.Write(messageBytes, 0, messageBytes.Length);
                pipeStream.Flush();
                pipeStream.WaitForPipeDrain();
            }

            string? popMessageToSend()
            {
                string? msg;
                lock (_lock_msg)
                {
                    msg = message;
                    message = null;
                }
                return msg;
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                    clearClientEvents();
                    clearServerEvents();
                }
                disposedValue = true;
            }
        }

        private void clearServerEvents()
        {
            OnServerStopped = null;
            OnServerStarted = null;
        }

        private void clearClientEvents()
        {
            OnNewClient = null;
            OnClientDisconnected = null;
            OnMessageSend = null;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
