namespace AfterburnerViewerServerWin.abconfig
{
    public interface IAfterburnerConfig
    {

        public string ConfigFile { get; }
        public bool IsConfigFileValid();
        
        public string? GetHistoryLogPath();
        public bool IsHistoryLogEnabled();
        public bool IsRecreateHistoryLog();
        public int? GetHistoryLogLimit();
    }
}