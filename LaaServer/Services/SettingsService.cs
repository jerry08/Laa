using System.Collections.Generic;
using System.Windows;
using Tyrrrz.Settings;

namespace LaaServer.Services
{
    public class SettingsService : SettingsManager
    {
        public double MainWindowHeight { get; set; }

        public double MainWindowWidth { get; set; }

        public double MainWindowTop { get; set; }

        public double MainWindowLeft { get; set; }

        public WindowState MainWindowState { get; set; }

        public bool IsAutoUpdateEnabled { get; set; } = true;

        public bool IsDarkModeEnabled { get; set; }
        
        public bool IsStartAppOnBootEnabled { get; set; }

        public SettingsService()
        {
            Configuration.StorageSpace = StorageSpace.Instance;
            Configuration.SubDirectoryPath = "";
            Configuration.FileName = "Settings.dat";
        }
    }
}
