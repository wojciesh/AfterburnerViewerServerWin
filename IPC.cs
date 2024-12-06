namespace AfterburnerViewerServerWin
{
    public class IpcServer : IDisposable
    {
        public event EventHandler? OnServerStarted;
        public event EventHandler? OnServerStopped;
        public event EventHandler<string>? OnError;
        public event EventHandler? OnNewClient;
        public event EventHandler? OnClientDisconnected;
        public event EventHandler<string>? OnMessageSend;

        public readonly string PipeName;

        private PipeServer? pipeServer = null;
        private bool disposedValue;

        public IpcServer(string pipeName)
        {
            if (string.IsNullOrEmpty(pipeName))
                throw new ArgumentException($"'{nameof(pipeName)}' cannot be null or empty.", nameof(pipeName));

            PipeName = pipeName;
            
            StartServer();
        }

        public void Write(string message)
        {
            pipeServer?.WriteToAllClients(message);
        }

        public void RestartServer()
        {
            StopServer();
            StartServer();
        }

        protected void StartServer()
        {
            pipeServer = createServer();
            pipeServer.Start();
        }

        protected void StopServer()
        {
            if (pipeServer != null)
            {
                pipeServer.Dispose();
                pipeServer = null;
            }
        }

        protected PipeServer createServer()
        {
            var pipeServer = new PipeServer(PipeName);

            pipeServer.OnServerStarted += (s, e) =>
            {
                OnServerStarted?.Invoke(this, EventArgs.Empty);
            };
            pipeServer.OnServerStopped += (s, e) =>
            {
                OnServerStopped?.Invoke(this, EventArgs.Empty);
            };
            pipeServer.OnNewClient += (s, e) =>
            {
                OnNewClient?.Invoke(this, EventArgs.Empty);
            };
            pipeServer.OnClientDisconnected += (s, e) =>
            {
                OnClientDisconnected?.Invoke(this, EventArgs.Empty);
            };
            pipeServer.OnMessageSend += (s, msg) =>
            {
                OnMessageSend?.Invoke(this, msg);
            };

            return pipeServer;
        }

        protected void error(string msg)
        {
            if (OnError == null) 
                throw new Exception(msg);
            else
                OnError.Invoke(this, msg);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;
            if (disposing)
            {
                StopServer();

                OnServerStarted = null;
                OnServerStopped = null;
                OnNewClient = null;
                OnClientDisconnected = null;
                OnMessageSend = null;
                OnError = null;
            }
            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
