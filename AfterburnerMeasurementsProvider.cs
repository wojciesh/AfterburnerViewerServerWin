
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text;

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

            Task.Run(async () =>
            {
                try
                {
                    if (!isValidSource()) 
                        throw new InvalidOperationException("Invalid source file");
                    Debug.Assert(Source != null);

                    isRunning = true;

                    using var sourceStream = new FileStream(Source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var sourceReader = new StreamReader(sourceStream, Encoding.UTF8);

                    sourceStream.Seek(0, SeekOrigin.End);

                    while (isRunning)
                    {
                        await Task.Delay(100);

                        if (String.IsNullOrEmpty(Source) || !IsUnreadDataInSource())
                            continue;

                        string? lastLine = await readNewLine(sourceReader);

                        if (!String.IsNullOrEmpty(lastLine))
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


                bool IsUnreadDataInSource()
                {
                    var lastModTime = File.GetLastWriteTimeUtc(Source);
                    if (lastModTime == oldLastModified)
                        return false;

                    oldLastModified = lastModTime;
                    return true;
                }

                static async Task<string?> readNewLine(StreamReader reader)
                {
                    string? line = null;
                    while (!reader.EndOfStream)
                    {
                        line = await reader.ReadLineAsync();
                    }
                    return line == null || line.Trim().Length == 0
                        ? null
                        : line;
                }
            });

            return true;
        }

        public void Stop()
        {
            isRunning = false;
        }

    }
}