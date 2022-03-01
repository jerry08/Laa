using Gress;
using InputSimulatorStandard;
using InputSimulatorStandard.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LaaServer;
using System.Windows.Input;
using LaaServer.Common.Services;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using Laa.Shared;
using LaaServer.ViewModels.Framework;
using LaaServer.ViewModels.Dialogs;
using Newtonsoft.Json;
using TouchPoint = Laa.Shared.TouchPoint;
using System.Threading.Tasks;
using System.Threading;

namespace LaaServer.ViewModels
{
    public class BluetoothViewModel : BaseViewModel
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly DialogManager _dialogManager;

        public string DeviceName { get; set; }

        public bool IsRunning { get { return BluetoothService != null; } }

        public bool IsOn { get; set; }
        
        ReceiverBluetoothService BluetoothService { get; set; }

        #region Commands
        private ICommand _toggledCommand;
        public ICommand ToggledCommand
        {
            get
            {
                return _toggledCommand ??
                    (_toggledCommand = new CommandHandler((s) => Toggled(), () => true));
            }
        }

        private ICommand _startCommand;
        public ICommand StartCommand
        {
            get
            {
                return _startCommand ??
                    (_startCommand = new CommandHandler((s) => Start(), () => CanStart));
            }
        }

        public bool CanStart
        {
            get
            {
                return !IsRunning;
            }
        }

        private ICommand _stopCommand;
        public ICommand StopCommand
        {
            get
            {
                return _stopCommand ??
                    (_stopCommand = new CommandHandler((s) => Stop(), () => CanStop));
            }
        }

        public bool CanStop
        {
            get
            {
                return IsRunning;
            }
        }

        private ICommand _restartCommand;
        public ICommand RestartCommand
        {
            get
            {
                return _restartCommand ??
                    (_restartCommand = new CommandHandler((s) => Restart(), () => CanRestart));
            }
        }

        public bool CanRestart
        {
            get
            {
                return true;
            }
        }

        private ICommand _shutDownCommand;
        public ICommand ShutDownCommand
        {
            get
            {
                return _shutDownCommand ??
                    (_shutDownCommand = new CommandHandler((s) => ShutDown(), () => true));
            }
        }
        #endregion

        public BluetoothViewModel(
            IViewModelFactory viewModelFactory,
            DialogManager dialogManager)
        {
            _viewModelFactory = viewModelFactory;
            _dialogManager = dialogManager;

            if (BluetoothRadio.Default != null)
            {
                DeviceName = BluetoothRadio.Default.Name;
                Start();
            }
        }

        private void Toggled()
        {
            //IsOn = !IsOn;

            if (IsOn)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        private void ShutDown()
        {
            Stop();
            App.IsExit = true;
            App.Current.MainWindow.Close();
        }

        private async void Start()
        {
            if (BluetoothService != null && BluetoothService.WasStarted)
            {
                return;
            }

            if (BluetoothRadio.Default == null)
            {
                IsOn = false;
                this.OnPropertyChanged(null);
                var dialog = _viewModelFactory.CreateMessageBoxViewModel("Laa", "Please ensure your bluetooth is turned.");
                await _dialogManager.ShowDialogAsync(dialog);
                return;
            }

            DeviceName = BluetoothRadio.Default.Name;

            var client = new BluetoothClient();
            var pairedDevices = client.PairedDevices.ToList();

            if (pairedDevices.Count <= 0)
            {
                IsOn = false;
                this.OnPropertyChanged(null);
                var dialog = _viewModelFactory.CreateMessageBoxViewModel("No paried devices found", "Please ensure your bluetooth is turned on and paired with your mobile device");
                await _dialogManager.ShowDialogAsync(dialog);
                return;
            }

            //new ReceiverBluetoothService().Start((s) => 
            //{
            //    
            //});

            BluetoothService = new ReceiverBluetoothService();
            BluetoothService.Start(EnqueueMessage);

            App.IsExit = false;

            IsOn = true;
            this.OnPropertyChanged(null);
        }

        private async void Restart()
        {
            //if (Server != null)
            //{
            //    Server.Restart();
            //}

            var dialog = _viewModelFactory.CreateMessageBoxViewModel("Laa", "Restart completed");
            await _dialogManager.ShowDialogAsync(dialog);
        }

        private void Stop()
        {
            App.IsExit = true;

            DeviceName = "";
            this.OnPropertyChanged(null);

            if (BluetoothService != null)
            {
                BluetoothService.Stop();
                BluetoothService = null;
            }
        }
    }
}