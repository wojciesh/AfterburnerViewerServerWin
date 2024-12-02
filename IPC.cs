using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterburnerViewerServerWin
{
    public class IpcServer : IDisposable
    {
        public readonly string PipeName;

        private readonly PipeServer pipeServer;

        private bool disposedValue;

        public event EventHandler? OnDisconnected;
        public event EventHandler<string>? OnError;
        public event EventHandler? OnNewClient;
        public event EventHandler? OnClientDisconnected;
        public event EventHandler<string>? OnMessageSend;

        public IpcServer(string pipeName)
        {
            if (string.IsNullOrEmpty(pipeName))
                throw new ArgumentException($"'{nameof(pipeName)}' cannot be null or empty.", nameof(pipeName));

            PipeName = pipeName;
            pipeServer = new PipeServer(PipeName);

            pipeServer.OnNewClient += (s, user) =>
            {
                OnNewClient?.Invoke(this, EventArgs.Empty);
            };
            pipeServer.OnClientDisconnected += (s, user) =>
            {
                OnClientDisconnected?.Invoke(this, EventArgs.Empty);
            };
            pipeServer.OnMessageSend += (s, msg) =>
            {
                OnMessageSend?.Invoke(this, msg);
            };
        }

        public void error(string msg)
        {
            if (OnError == null) 
                throw new Exception(msg);
            else
                OnError.Invoke(this, msg);
        }

        public void Start()
        {
            pipeServer.Run();
        }

        public void Write(string message)
        {
            pipeServer.WriteToAllClients(message);
        }

        public void Stop()
        {
            pipeServer.Stop();
            OnDisconnected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;
            if (disposing)
            {
                Stop();
                pipeServer.Dispose();
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
