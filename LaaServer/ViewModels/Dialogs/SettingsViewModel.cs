using LaaServer.Services;
using LaaServer.ViewModels.Framework;

namespace LaaServer.ViewModels.Dialogs
{
    public class SettingsViewModel : DialogScreen
    {
        private readonly SettingsService _settingsService;

        public bool IsAutoUpdateEnabled
        {
            get => _settingsService.IsAutoUpdateEnabled;
            set => _settingsService.IsAutoUpdateEnabled = value;
        }

        public bool IsDarkModeEnabled
        {
            get => _settingsService.IsDarkModeEnabled;
            set => _settingsService.IsDarkModeEnabled = value;
        }

        public SettingsViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }
    }
}
