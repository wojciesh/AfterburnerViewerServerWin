
using System.Diagnostics;

namespace AfterburnerViewerServerWin
{
    public class AfterburnerMeasurementsProvider : IMeasurementsProvider, IDisposable
    {
        public event EventHandler<String>? OnMeasurement;
        public event EventHandler<String>? OnError;

        public string Source { get; private set; } = String.Empty;
        
        private volatile bool isRunning;
        private FileSystemWatcher? sourceWatcher;
        private bool disposedValue;


        public bool Start(string source)
        {
            try
            {
                if (!IsValidSource(source))
                    throw new InvalidOperationException("Invalid source file");
                
                if (isRunning)
                    throw new InvalidOperationException("Already started");

                Source = source;

                InitSourceMonitoring();

                isRunning = true;
            }
            catch (Exception e) 
            {
                OnError?.Invoke(this, e.Message);
                return false;
            }
            return true;
        }

        public bool Stop(bool notifyOnError = true)
        {
            try
            {
                if (!isRunning)
                    throw new InvalidOperationException("Cannot stop, not running");

                DestroySourceMonitoring();

                isRunning = false;
            }
            catch (Exception e)
            {
                if (notifyOnError) 
                    OnError?.Invoke(this, e.Message);

                return false;
            }
            return true;
        }


        private void InitSourceMonitoring()
        {
            if (sourceWatcher != null) 
                throw new InvalidOperationException("Already monitoring");

            Debug.Assert(Source != null);
            sourceWatcher = new FileSystemWatcher(Path.GetDirectoryName(Source)
                ?? throw new InvalidOperationException("Invalid source file path"))
            {
                Filter = Path.GetFileName(Source),
                NotifyFilter = NotifyFilters.LastWrite
            };
            sourceWatcher.Changed += OnSourceWatcher_Changed;
            sourceWatcher.EnableRaisingEvents = true;
        }

        private void DestroySourceMonitoring()
        {
            if (sourceWatcher == null)
                throw new InvalidOperationException("Not monitoring");

            sourceWatcher.EnableRaisingEvents = false;
            sourceWatcher.Changed -= OnSourceWatcher_Changed;
            sourceWatcher.Dispose();
            sourceWatcher = null;
        }

        private async void OnSourceWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            string? lastLine = await getLastLineAsync();
            if (!String.IsNullOrEmpty(lastLine))
                OnMeasurement?.Invoke(this, lastLine);


            async Task<string?> getLastLineAsync()
            {
                const int defaultBuffSize = 4096;
                using var fs = new FileStream(Source, FileMode.Open, FileAccess.Read, FileShare.Read, defaultBuffSize);
                using var sr = new StreamReader(fs, true);

                fs.Seek(-defaultBuffSize, SeekOrigin.End);

                string? line = null;
                while (!sr.EndOfStream)
                    line = await sr.ReadLineAsync();

                return line == null || line.Trim().Length == 0
                    ? null
                    : line;
            }
        }


        public bool IsValidSource(string? source)
        {
            return !String.IsNullOrWhiteSpace(source) && File.Exists(source);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                    OnMeasurement = null;
                    OnError = null;
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