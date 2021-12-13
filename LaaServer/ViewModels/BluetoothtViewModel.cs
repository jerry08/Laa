using Gress;
using InputSimulatorStandard;
using InputSimulatorStandard.Native;
using LaaServer.Common.Network;
using NetFwTypeLib;
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
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using Laa.Shared;

namespace LaaServer.ViewModels
{
    public class BluetoothViewModel : PropertyChangedBase
    {
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

        public BluetoothViewModel()
        {
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
            App.IsExit = true;
            Stop();
            var currentProcess = Process.GetCurrentProcess();
            List<Process> processes = Process.GetProcesses()
                .Where(x => x.ProcessName == currentProcess.ProcessName).ToList();
            processes.ForEach(x => x.Kill());
        }

        string prevMessage = "";
        string prevBkMessage = "";
        InputSimulator simulator = new InputSimulator();
        private void MessageReceived(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
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

        private void Start()
        {
            if (BluetoothRadio.Default == null)
            {
                IsOn = false;
                this.OnPropertyChanged(null);
                MessageBox.Show("Please ensure your bluetooth is turned.", "Laa", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DeviceName = BluetoothRadio.Default.Name;

            var client = new BluetoothClient();
            var pairedDevices = client.PairedDevices.ToList();

            if (pairedDevices.Count <= 0)
            {
                IsOn = false;
                this.OnPropertyChanged(null);
                MessageBox.Show("Please ensure your bluetooth is turned on and paired with your mobile device", "No paried devices found", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void Restart()
        {
            //if (Server != null)
            //{
            //    Server.Restart();
            //}

            MessageBox.Show("Restart completed");
        }

        private void Stop()
        {
            App.IsExit = true;

            if (BluetoothService != null)
            {
                BluetoothService.Stop();
                BluetoothService = null;
            }
        }
    }
}
