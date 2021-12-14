using Gress;
using LaaServer.Services;
using System.Windows.Input;
using LaaServer.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;
using LaaServer.Views.Dialogs;

namespace LaaServer.ViewModels
{
    public class RootViewModel : PropertyChangedBase
    {
        public ISnackbarMessageQueue Notifications { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));

        public IProgressManager ProgressManager { get; } = new ProgressManager();

        private readonly UpdateService _updateService;

        private readonly SettingsService _settingsService;

        public string DisplayName { get; set; }

        #region Commands
        private ICommand _onViewLoadedCommand;
        public ICommand OnViewLoadedCommand => _onViewLoadedCommand ??= new CommandHandler((s) => OnViewLoaded(), () => true);

        public bool CanExecute => true;
        #endregion

        public RootViewModel()
        {
            _settingsService = new SettingsService();
            _updateService = new UpdateService(_settingsService);

            // Title
            DisplayName = $"{App.Name} v{App.VersionString}";

            //App.Current.MainWindow.Closing += delegate { OnClose(); };
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                // Check for updates
                var updateVersion = await _updateService.CheckForUpdatesAsync();
                if (updateVersion is null)
                    return;

                // Notify user of an update and prepare it
                Notifications.Enqueue($"Downloading update to {App.Name} v{updateVersion}...");
                await _updateService.PrepareUpdateAsync(updateVersion);

                // Prompt user to install update (otherwise install it when application exits)
                Notifications.Enqueue(
                    "Update has been downloaded and will be installed when you exit",
                    "INSTALL NOW", () =>
                    {
                        _updateService.FinalizeUpdate(true);
                        if (MessageBox.Show("Do you want to install it now?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information)
                            == MessageBoxResult.Yes)
                        {
                            App.Current.MainWindow.Close();
                        }
                    });
            }
            catch
            {
                // Failure to update shouldn't crash the application
                Notifications.Enqueue("Failed to perform application update");
            }
        }

        public void OnClose()
        {
            _settingsService.Save();

            _updateService.FinalizeUpdate(false);
        }

        public async void OnViewLoaded()
        {
            _settingsService.Load();

            if (_settingsService.IsDarkModeEnabled)
            {
                App.SetDarkTheme();
            }
            else
            {
                App.SetLightTheme();
            }

            await CheckForUpdatesAsync();
        }

        public void ShowSettings()
        {
            var window = new SettingsView();
            window.ShowDialog();
        }
    }
}