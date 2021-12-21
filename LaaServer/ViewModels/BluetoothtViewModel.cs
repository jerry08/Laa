using Gress;
using InputSimulatorStandard;
using InputSimulatorStandard.Native;
using LaaServer.Common.Network;
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

namespace LaaServer.ViewModels
{
    public class BluetoothViewModel : PropertyChangedBase
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

        int _mouseScale = 2;
        TouchPoint TouchPointFrom;
        /*TouchPoint TouchPointTo;
        Task MouseTask;

        void StartMovingMouse()
        {
            if (MouseTask != null && !MouseTask.IsCompleted)
            {
                return;
            }

            MouseTask = Task.Run(() => 
            {
                while (TouchPointFrom != null && TouchPointTo != null)
                {
                    int difX = TouchPointFrom.X - TouchPointFrom.X;
                    int difY = TouchPointFrom.Y - TouchPointFrom.Y;
                    if (difX > 10 && difX < -10)
                    {
                        simulator.Mouse.MoveMouseBy(TouchPointTo.X - TouchPointFrom.X, TouchPointTo.Y - TouchPointFrom.Y);
                        TouchPointTo = TouchPointFrom;
                    }
                }
            });
        }*/

        string prevMessage = "";
        string prevBkMessage = "";
        InputSimulator simulator = new InputSimulator();
        private void MessageReceived(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (message.EndsWith(LaaConstants.MouseLocationHash))
            {
                string val = message.Replace(LaaConstants.MouseLocationHash, "");
                var point = JsonConvert.DeserializeObject<TouchPoint>(val);
                if (TouchPointFrom == null)
                {
                    TouchPointFrom = point;
                }
                else
                {
                    //int difX = point.X - TouchPointFrom.X;
                    //int difY = point.Y - TouchPointFrom.Y;
                    //if (difX > 5 || difX < -5 || difY > 5 || difY < -5)
                    //{
                    //    while (point.X != TouchPointFrom.X)
                    //    {
                    //        simulator.Mouse.MoveMouseBy(point.X - TouchPointFrom.X, point.Y - TouchPointFrom.Y);
                    //    }
                    //    
                    //    TouchPointFrom = point;
                    //}

                    while (point.X != TouchPointFrom.X || point.Y != TouchPointFrom.Y)
                    {
                        //int x = point.X - TouchPointFrom.X;
                        //int y = point.Y - TouchPointFrom.Y;

                        int x = 0;
                        int y = 0;

                        if (point.X != TouchPointFrom.X)
                        {
                            if ((TouchPointFrom.X - point.X) < 0)
                            {
                                TouchPointFrom.X += 1;
                                x = 1;
                            }
                            else
                            {
                                TouchPointFrom.X += -1;
                                x = -1;
                            }
                        }

                        if (point.Y != TouchPointFrom.Y)
                        {
                            if ((TouchPointFrom.Y - point.Y) < 0)
                            {
                                TouchPointFrom.Y += 1;
                                y = 1;
                            }
                            else
                            {
                                TouchPointFrom.Y += -1;
                                y = -1;
                            }
                        }

                        simulator.Mouse.MoveMouseBy(x * _mouseScale, y * _mouseScale);
                    }

                    TouchPointFrom = point;
                }
                
                if (point.TouchActionType == TouchActionType.Released)
                {
                    TouchPointFrom = null;
                }

                return;
            }

            if (message.EndsWith(LaaConstants.SecondHash))
            {
                if (prevMessage.EndsWith(LaaConstants.FirstHash))
                {
                    prevMessage = message;
                    return;
                }
            }

            prevMessage = message;

            message = message.Replace(LaaConstants.SecondHash, "").Replace(LaaConstants.FirstHash, "");

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (message.Contains("backspace"))
            {
                if (message.EndsWith(LaaConstants.SecondBkHash))
                {
                    if (prevBkMessage.EndsWith(LaaConstants.FirstBkHash))
                    {
                        prevBkMessage = message;
                        return;
                    }
                }

                prevBkMessage = message;

                int backspaces = message.Split("backspace").Length;
                if (backspaces > 2)
                {
                    simulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
                }

                simulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
            }
            else
            {
                simulator.Keyboard.TextEntry(message);
            }
        }

        private async void Start()
        {
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
            BluetoothService.Start(MessageReceived);

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
