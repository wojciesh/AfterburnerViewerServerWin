namespace AfterburnerViewerServerWin
{
    public interface IAppConfig
    {
        string configVersion { get; set; }
        string sourceFile { get; set; }
        string abConfigFile { get; set; }
    }
}