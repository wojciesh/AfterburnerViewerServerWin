namespace AfterburnerViewerServerWin
{
    public interface IAppConfig
    {
        string ConfigVersion { get; set; }
        string Source { get; set; }
    }
}