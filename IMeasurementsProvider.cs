﻿namespace AfterburnerViewerServerWin
{
    public interface IMeasurementsProvider : IDisposable
    {
        event EventHandler<List<AfterburnerMeasurement>>? OnNewMeasurements;
        event EventHandler<string>? OnError;
        
        string Source { get; }

        bool IsValidSource(string? source);
        bool Start(string source);
        bool Stop(bool notifyOnError = true);
    }
}