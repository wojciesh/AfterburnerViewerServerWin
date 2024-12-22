namespace AfterburnerViewerServerWin
{
    public interface IAppConfig
    {
        string configVersion { get; set; }
        string source { get; set; }
        string abConfigFile { get; set; }
    }
}