using Gress;
using InputSimulatorStandard;
using InputSimulatorStandard.Native;
using LaaServer.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LaaServer;
using System.Windows.Input;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using LaaServer.Common.Services;
using Laa.Shared;
using LaaServer.ViewModels.Framework;
using LaaServer.ViewModels.Dialogs;
using TouchPoint = Laa.Shared.TouchPoint;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

namespace LaaServer.ViewModels
{
    public class WifiViewModel : BaseViewModel
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly DialogManager _dialogManager;

        public string IpAddress { get; private set; }

        public bool IsRunning { get { return Server != null; } }

        public bool IsOn { get; set; }

        //private ChatServer Server { get; set; }
        private EchoServer Server { get; set; }

        #region Commands
        private ICommand _toggledCommand;
        public ICommand ToggledCommand => _toggledCommand ??= new CommandHandler((s) => Toggled(), () => true);

        private ICommand _startCommand;
        public ICommand StartCommand => _startCommand ??= new CommandHandler((s) => Start(s), () => CanStart);

        public bool CanStart => !IsRunning;

        private ICommand _stopCommand;
        public ICommand StopCommand => _stopCommand ??= new CommandHandler((s) => Stop(), () => CanStop);

        public bool CanStop => IsRunning;

        private ICommand _restartCommand;
        public ICommand RestartCommand => _restartCommand ??= new CommandHandler((s) => Restart(), () => CanRestart);

        public bool CanRestart => true;

        private ICommand _shutDownCommand;
        public ICommand ShutDownCommand => _shutDownCommand ??= new CommandHandler((s) => ShutDown(), () => true);
        #endregion

        public WifiViewModel(
            IViewModelFactory viewModelFactory,
            DialogManager dialogManager)
        {
            _viewModelFactory = viewModelFactory;
            _dialogManager = dialogManager;
        }

        private void Toggled()
        {
            //IsOn = !IsOn;

            if (IsOn)
            {
                Start(null);
            }
            else
            {
                Stop();
            }
        }

        //private void ShutDown()
        //{
        //    App.IsExit = true;
        //    Stop();
        //    var currentProcess = Process.GetCurrentProcess();
        //    List<Process> processes = Process.GetProcesses()
        //        .Where(x => x.ProcessName == currentProcess.ProcessName).ToList();
        //    processes.ForEach(x => x.Kill());
        //}

        private void ShutDown()
        {
            Stop();
            App.IsExit = true;
            App.Current.MainWindow.Close();
        }

        private async void Start(object parameter)
        {
            bool autoStart = false;

            if (parameter is true)
            {
                autoStart = true;
            }

            IpAddress = NetworkHelper.GetAllLocalIPv4(NetworkInterfaceType.Wireless80211).FirstOrDefault();
            OnPropertyChanged(null);

            int port = LaaConstants.WifiPort;

            if (!FirewallHelper.IsPortOpen(port))
            {
                if (autoStart)
                {
                    return;
                }

                if (App.IsAdministrator())
                {
                    FirewallHelper.AddOutboundRule();
                }
                else
                {

#if DEBUG
                    throw new Exception($"The firewall port ({LaaConstants.WifiPort}) must be opened. Please restart this app as Administrator.");
#else
                    if (!App.IsAdministrator())
                    {
                        MessageBox.Show($"The firewall port ({LaaConstants.WifiPort}) must be opened. The app will restart as Admin", "Laa", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        App.RestartAsAdmin();
                        return;
                    }
#endif
                }
            }

            if (string.IsNullOrEmpty(IpAddress))
            {
                IsOn = false;
                OnPropertyChanged(null);
                var dialog = _viewModelFactory.CreateMessageBoxViewModel("Error", "Connection not found");
                await _dialogManager.ShowDialogAsync(dialog);
                return;
            }
            
            try
            {
                Server = new EchoServer(IPAddress.Parse(IpAddress), port);
                Server.OnMessageReceived += Server_OnMessageReceived;
                Server.Start();

                IsOn = true;
            }
            catch (Exception e)
            {
                IsOn = false;
                OnPropertyChanged(null);
                var dialog = _viewModelFactory.CreateMessageBoxViewModel("Error", e.Message);
                await _dialogManager.ShowDialogAsync(dialog);
            }

            App.IsExit = false;
        }

        private void Server_OnMessageReceived(object sender, string e)
        {
            EnqueueMessage(e);
        }

        private async void Restart()
        {
            if (Server != null)
            {
                Server.Restart();
            }

            var dialog = _viewModelFactory.CreateMessageBoxViewModel("", "Restart completed");
            await _dialogManager.ShowDialogAsync(dialog);
        }

        private void Stop()
        {
            App.IsExit = true;

            if (Server != null)
            {
                Server.Stop();
                Server = null;
            }

            IpAddress = "";
            OnPropertyChanged(null);
        }
    }
}