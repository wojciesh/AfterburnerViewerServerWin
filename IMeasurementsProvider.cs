namespace AfterburnerViewerServerWin
{
    public interface IMeasurementsProvider
    {
        event EventHandler<String>? OnMeasurement;
        event EventHandler<String>? OnError;
        
        public string? Source { get; set; }

        public bool isValidSource();
        bool Start();
        void Stop();
    }
}