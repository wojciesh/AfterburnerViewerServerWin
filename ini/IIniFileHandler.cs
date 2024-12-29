namespace AfterburnerViewerServerWin.ini
{
    public interface IIniFileHandler
    {
        string GetValue(string section, string key, string filePath);
        void SetValue(string section, string key, string value, string filePath);
    }

}
