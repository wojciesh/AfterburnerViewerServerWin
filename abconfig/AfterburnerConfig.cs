using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterburnerViewerServerWin.abconfig
{
    public class AfterburnerConfig : IAfterburnerConfig
    {
        public static readonly string DEFAULT_CONFIG_FILE = @"C:\Program Files (x86)\MSI Afterburner\Profiles\MSIAfterburner.cfg";

        public string ConfigFile { get; }


        public AfterburnerConfig(string? configFile)
        {
            ConfigFile = configFile ?? DEFAULT_CONFIG_FILE;

            if (!IsConfigFileValid())
                throw new ArgumentException("Invalid config file");
        }

        public bool IsConfigFileValid()
        {
            return File.Exists(ConfigFile);
        }

        public string? GetHistoryLogPath() => GetSetting<string?>("LogPath");

        public bool IsHistoryLogEnabled() => (GetSetting<int?>("EnableLog") ?? 0) == 1;

        public bool IsRecreateHistoryLog() => (GetSetting<int?>("RecreateLog") ?? 0) == 1;

        public int? GetHistoryLogLimit() => GetSetting<int?>("LogLimit");

        protected T? GetSetting<T>(string keyName)
        {
            return default;
        }

        /*
        [Settings]
        LogPath=D:\HardwareMonitoring.hml
        EnableLog=1
        RecreateLog=1
        LogLimit=0
        */
    }
}
