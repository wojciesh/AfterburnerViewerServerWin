using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterburnerViewerServerWin
{
    internal class AfterburnerConfig
    {
        public string ConfigFile { get; set; } = @"C:\Program Files (x86)\MSI Afterburner\Profiles\MSIAfterburner.cfg";

        public bool IsConfigFileValid()
        {
            return File.Exists(ConfigFile);
        }

        /*
        [Settings]
        Views=
        AttachMonitoringWindow=1
        HideMonitoring=0
        MonitoringWindowOnTop=1
        LogPath=D:\HardwareMonitoring.hml
        EnableLog=1
        RecreateLog=1
        LogLimit=0
         */
        //public 
    }
}
