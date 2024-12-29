using System.Text;
using System.Runtime.InteropServices;

namespace AfterburnerViewerServerWin.ini
{
    public class WinIniFileHandler : IIniFileHandler
    {
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]
        public static extern int GetKeyValueA(string strSection,
                                      string strKeyName,
                                      string strEmpty,
                                      StringBuilder RetVal,
                                      int nSize,
                                      string strFilePath);

        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString")]
        public static extern long WriteValueA(string strSection,
                                       string strKeyName,
                                       string strValue,
                                       string strFilePath);

        public string GetValue(string section, string key, string filePath)
        {
            StringBuilder temp = new StringBuilder(255);
            GetKeyValueA(section, key, string.Empty, temp, 255, filePath);
            return temp.ToString();
        }

        public void SetValue(string section, string key, string value, string filePath)
        {
            WriteValueA(section, key, value, filePath);
        }
    }
}
