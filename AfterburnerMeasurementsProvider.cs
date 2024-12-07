
using System.Diagnostics.Metrics;

namespace AfterburnerViewerServerWin
{
    public class AfterburnerMeasurementsProvider : IMeasurementsProvider
    {

        public event EventHandler<String>? OnMeasurement;
        public event EventHandler<String>? OnError;

        public string? Source { get; set; }
        
        private volatile bool isRunning;
        private DateTime oldLastModified = DateTime.MinValue;

        public AfterburnerMeasurementsProvider() { }

        public AfterburnerMeasurementsProvider(string? source)
        {
            Source = source;
        }

        public static bool isValidSource(string? source)
        {
            return !String.IsNullOrWhiteSpace(source) && File.Exists(source);
        }

        public bool isValidSource()
        {
            return AfterburnerMeasurementsProvider.isValidSource(Source);
        }

        public bool Start()
        {
            if (isRunning)
            {
                OnError?.Invoke(this, "Already started");
                return false;
            }

            if (!isValidSource())
            {
                OnError?.Invoke(this, "Invalid source file: " + Source);
                return false;
            }

            Task.Run(() =>
            {
                try
                {
                    isRunning = true;
                    while (isRunning)
                    {
                        Thread.Sleep(100);

                        if (String.IsNullOrEmpty(Source))
                            continue;

                        var lastModTime = getLastModified();

                        if (lastModTime == oldLastModified)
                            continue;

                        oldLastModified = lastModTime;

                        using var fs = new FileStream(Source, FileMode.Open, FileAccess.Read, FileShare.Read);
                        fs.Seek(-3000, SeekOrigin.End);
                        using var sr = new StreamReader(fs);

                        List<string> lines = new();
                        while (isRunning)
                        {
                            string? line = sr.ReadLine();
                            if (line == null)
                                break;

                            line = line.Trim();
                            if (String.IsNullOrEmpty(line))
                                break;

                            lines.Add(line);
                        }

                        var lastLine = lines.Last();

                        if (String.IsNullOrEmpty(lastLine))
                            continue;

                        OnMeasurement?.Invoke(this, lastLine);
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, ex.Message);
                }
                finally
                {
                    isRunning = false;
                }
            });

            return true;
        }

        private DateTime getLastModified() => File.GetLastWriteTimeUtc(Source);

        public void Stop()
        {
            isRunning = false;
        }
    }
}