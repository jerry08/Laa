using Gress;
using LaaServer.Services;
using System.Windows.Input;
using LaaServer.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;
using LaaServer.Views.Dialogs;
using LaaServer.ViewModels.Framework;
using System.IO;

namespace LaaServer.ViewModels
{
    public class RootViewModel : PropertyChangedBase
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly DialogManager _dialogManager;
        private readonly SettingsService _settingsService;
        private readonly UpdateService _updateService;

        public ISnackbarMessageQueue Notifications { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));

        public IProgressManager ProgressManager { get; } = new ProgressManager();

        public string DisplayName { get; set; }

        #region Commands
        private ICommand _onViewLoadedCommand;
        public ICommand OnViewLoadedCommand => _onViewLoadedCommand ??= new CommandHandler((s) => OnViewLoaded(), () => true);

        private ICommand _onViewSizeLocationChangedCommand;
        public ICommand OnViewSizeLocationChangedCommand => _onViewSizeLocationChangedCommand ??= new CommandHandler((s) => OnViewSizeLocationChanged(), () => true);

        private ICommand _bthButtonCommand;
        public ICommand BthButtonCommand => _bthButtonCommand ??= new CommandHandler((s) => NavigateBtnPage(), () => true);

        private ICommand _wifiButtonCommand;
        public ICommand WifiButtonCommand => _wifiButtonCommand ??= new CommandHandler((s) => NavigateWifiPage(), () => true);

        private ICommand _showSettingsCommand;
        public ICommand ShowSettingsCommand => _showSettingsCommand ??= new CommandHandler((s) => ShowSettings(), () => true);

        public bool CanExecute => true;
        #endregion

        public RootViewModel(
            IViewModelFactory viewModelFactory, 
            DialogManager dialogManager,
            SettingsService settingsService, 
            UpdateService updateService)
        {
            _dialogManager = dialogManager;
            _viewModelFactory = viewModelFactory;
            _settingsService = settingsService;
            _updateService = updateService;

            // Title
            DisplayName = $"{App.DisplayName} v{App.VersionString}";

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

        public void OnViewSizeLocationChanged()
        {
            _settingsService.MainWindowHeight = Application.Current.MainWindow.Height;
            _settingsService.MainWindowWidth = Application.Current.MainWindow.Width;
            _settingsService.MainWindowTop = Application.Current.MainWindow.Top;
            _settingsService.MainWindowLeft = Application.Current.MainWindow.Left;
            _settingsService.MainWindowState = Application.Current.MainWindow.WindowState;
        }

        public void SizeToFit()
        {
            if (_settingsService.MainWindowHeight > SystemParameters.VirtualScreenHeight)
            {
                _settingsService.MainWindowHeight = SystemParameters.VirtualScreenHeight;
            }

            if (_settingsService.MainWindowWidth > SystemParameters.VirtualScreenWidth)
            {
                _settingsService.MainWindowWidth = SystemParameters.VirtualScreenWidth;
            }
        }

        public void MoveIntoView()
        {
            if (_settingsService.MainWindowTop + _settingsService.MainWindowWidth / 2 >
                SystemParameters.VirtualScreenHeight)
            {
                _settingsService.MainWindowTop = SystemParameters.VirtualScreenHeight -
                    _settingsService.MainWindowHeight;
            }

            if (_settingsService.MainWindowLeft + _settingsService.MainWindowWidth / 2 >
                     SystemParameters.VirtualScreenWidth)
            {
                _settingsService.MainWindowLeft = SystemParameters.VirtualScreenWidth -
                    _settingsService.MainWindowWidth;
            }

            if (_settingsService.MainWindowTop < 0)
            {
                _settingsService.MainWindowTop = 0;
            }

            if (_settingsService.MainWindowLeft < 0)
            {
                _settingsService.MainWindowLeft = 0;
            }
        }

        public async void OnViewLoaded()
        {
            _settingsService.Load();

            RestoreWindow();

            await CheckForUpdatesAsync();
        }

        public void RestoreWindow()
        {
            if (_settingsService.IsDarkModeEnabled)
            {
                App.SetDarkTheme();
            }
            else
            {
                App.SetLightTheme();
            }

            if (File.Exists(_settingsService.FullFilePath))
            {
                SizeToFit();
                MoveIntoView();

                if (!_settingsService.IsSaved)
                {
                    _settingsService.Save();
                }

                Application.Current.MainWindow.Height = _settingsService.MainWindowHeight;
                Application.Current.MainWindow.Width = _settingsService.MainWindowWidth;
                Application.Current.MainWindow.Top = _settingsService.MainWindowTop;
                Application.Current.MainWindow.Left = _settingsService.MainWindowLeft;
                Application.Current.MainWindow.WindowState = _settingsService.MainWindowState;
            }
            else
            {
                CenterWindowOnScreen();
            }
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = Application.Current.MainWindow.Width;
            double windowHeight = Application.Current.MainWindow.Height;
            Application.Current.MainWindow.Left = (screenWidth / 2) - (windowWidth / 2);
            Application.Current.MainWindow.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        public void NavigateBtnPage()
        {
            App.NavigationService.Navigate(new BluetoothPage() 
            { 
                DataContext = _viewModelFactory.CreateBluetoothViewModel()
            });
        }
        
        public void NavigateWifiPage(bool autoStart = false)
        {
            var viewModel = _viewModelFactory.CreateWifiViewModel();

            if (autoStart)
            {
                viewModel.StartCommand.Execute(autoStart);
            }

            App.NavigationService.Navigate(new WifiPage()
            {
                DataContext = viewModel
            });
        }

        public async void ShowSettings()
        {
            var dialog = _viewModelFactory.CreateSettingsViewModel();
            await _dialogManager.ShowDialogAsync(dialog);
        }
    }
}