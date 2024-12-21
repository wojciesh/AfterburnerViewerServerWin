using AfterburnerViewerServerWin.ini;
using System.ComponentModel;
using System.Globalization;

namespace AfterburnerViewerServerWin.abconfig
{
    public class AfterburnerConfig : IAfterburnerConfig
    {
        public static readonly string DEFAULT_CONFIG_FILE = @"C:\Program Files (x86)\MSI Afterburner\Profiles\MSIAfterburner.cfg";

        public string ConfigFile { get; }

        private IIniFileHandler iniFileHandler = new WinIniFileHandler();

        /**
         * @param configFile Path to the Afterburner config file or null to use the default one
         * @throws ArgumentException if the config file is invalid
         */
        public AfterburnerConfig(string? configFile)
        {
            ConfigFile = configFile ?? DEFAULT_CONFIG_FILE;

            if (!IsConfigFileValid())
                throw new ArgumentException("Invalid config file");
        }

        public bool IsConfigFileValid() => File.Exists(ConfigFile);

        public string? GetHistoryLogPath() => GetSetting<string?>("LogPath");

        public bool IsHistoryLogEnabled() => (GetSetting<int?>("EnableLog") ?? 0) == 1;

        public bool IsRecreateHistoryLog() => (GetSetting<int?>("RecreateLog") ?? 0) == 1;

        public int? GetHistoryLogLimit() => GetSetting<int?>("LogLimit");

        protected T? GetSetting<T>(string settingKeyName)
        {
            T? defaultValue = default;

            if (string.IsNullOrEmpty(settingKeyName))
                return defaultValue;

            return GetSetting("Settings", settingKeyName, defaultValue);
        }

        protected T? GetSetting<T>(string section, string key, T? defaultValue = default)
        {
            string value = iniFileHandler.GetValue("Settings", key, ConfigFile);

            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null && converter.IsValid(value))
                {
                    return (T?)converter.ConvertFromString(null, CultureInfo.InvariantCulture, value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting value '{value}' to type {typeof(T)}: {ex.Message}");
            }

            return defaultValue;
        }
    }
}
