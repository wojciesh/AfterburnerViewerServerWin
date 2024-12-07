namespace AfterburnerViewerServerWin
{
    public interface IMeasurementsProvider : IDisposable
    {
        event EventHandler<String>? OnMeasurement;
        event EventHandler<String>? OnError;
        
        string Source { get; }

        bool IsValidSource(string? source);
        bool Start(string source);
        bool Stop(bool notifyOnError = true);
    }
}