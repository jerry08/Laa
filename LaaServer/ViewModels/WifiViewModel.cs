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
using Laa.Shared;
using LaaServer.ViewModels.Framework;
using LaaServer.ViewModels.Dialogs;
using TouchPoint = Laa.Shared.TouchPoint;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace LaaServer.ViewModels
{
    public class WifiViewModel : PropertyChangedBase
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly DialogManager _dialogManager;

        public string IpAddress { get; private set; }

        public bool IsRunning { get { return Server != null; } }

        public bool IsOn { get; set; }

        private ChatServer Server { get; set; }

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
                Start();
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

        static int _mouseScale = 3;
        static TouchPoint TouchPointFrom;
        static TouchPoint TouchPointTo;
        static Task MouseTask;

        static void StartMovingMouse(TouchPoint point)
        {
            decimal steps = 10;

            if (MouseTask != null && !MouseTask.IsCompleted)
            {
                TouchPointTo = point;
                return;
            }

            if (TouchPointFrom == null)
            {
                if (point.TouchActionType != TouchActionType.Released)
                {
                    TouchPointFrom = point;
                }

                return;
            }

            TouchPointTo = point;

            MouseTask = Task.Run(async () =>
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

                while (TouchPointTo.TouchActionType != TouchActionType.Released)
                {
                    //int x = point.X - TouchPointFrom.X;
                    //int y = point.Y - TouchPointFrom.Y;

                    int x = 0;
                    int y = 0;

                    int diffX = TouchPointTo.X - TouchPointFrom.X;
                    int diffY = TouchPointTo.Y - TouchPointFrom.Y;

                    if (diffX > -10 && diffX < 10)
                    {
                        TouchPointTo.X = TouchPointFrom.X;
                    }

                    if (diffY > -10 && diffY < 10)
                    {
                        TouchPointTo.Y = TouchPointFrom.Y;
                    }

                    decimal x2 = diffX;
                    x = (int)Math.Round(x2 / steps);
                    TouchPointFrom.X += x;

                    decimal y2 = diffY;
                    y = (int)Math.Round(y2 / steps);
                    TouchPointFrom.Y += y;

                    //this.DeviceName = $"x: {x * _mouseScale},  y: {y * _mouseScale}";
                    //this.OnPropertyChanged(null);
                    simulator.Mouse.MoveMouseBy(x * _mouseScale, y * _mouseScale);

                    //Thread.Sleep(2);
                    await Task.Delay(1);
                }

                //TouchPointFrom = TouchPointTo;

                if (TouchPointTo.TouchActionType == TouchActionType.Released)
                {
                    TouchPointFrom = null;
                }
            });
        }

        static string prevMessage = "";
        static string prevBkMessage = "";
        static InputSimulator simulator = new InputSimulator();
        public static void MessageReceived(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (message.EndsWith(LaaConstants.MouseLocationHash))
            {
                string val = "[" + message.Replace(LaaConstants.MouseLocationHash, "")
                    .Replace("}{", "},{") + "]";

                var points = JsonConvert.DeserializeObject<List<TouchPoint>>(val);
                for (int i = 0; i < points.Count; i++)
                {
                    StartMovingMouse(points[i]);
                }

                return;
            }

            var splitArr = new string[]
            {
                LaaConstants.FirstHash,
                LaaConstants.SecondHash,
                LaaConstants.FirstBkHash,
                LaaConstants.SecondBkHash
            };

            //string[] messages = message.Split(splitArr, StringSplitOptions.None);
            //string[] messages = Regex.Split(message, LaaConstants.FirstHash);
            string[] messages = Regex.Split(message, $@"({LaaConstants.FirstHash}|{LaaConstants.SecondHash}|{LaaConstants.FirstBkHash}|{LaaConstants.SecondBkHash})");

            messages = messages.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            for (int i = 0; i < messages.Length; i++)
            {
                if (!splitArr.Contains(messages[i]))
                {
                    PerformAction(messages[i] + messages[i + 1]);
                }
            }
        }

        static void PerformAction(string message)
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

        private async void Start()
        {
            IpAddress = NetworkHelper.GetAllLocalIPv4(NetworkInterfaceType.Wireless80211).FirstOrDefault();
            this.OnPropertyChanged(null);

            int port = 9091;

            if (!FirewallHelper.IsPortOpen(port))
            {
                if (App.IsAdministrator())
                {
                    FirewallHelper.AddOutboundRule();
                }
                else
                {

#if DEBUG
                    throw new Exception("The firewall port (9091) must be opened. Please restart this app as Administrator.");
#else
                    if (!App.IsAdministrator())
                    {
                        MessageBox.Show("The firewall port (9091) must be opened. The app will restart as Admin", "Laa", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        App.RestartAsAdmin();
                        return;
                    }
#endif
                }
            }

            if (string.IsNullOrEmpty(IpAddress))
            {
                IsOn = false;
                this.OnPropertyChanged(null);
                var dialog = _viewModelFactory.CreateMessageBoxViewModel("Error", "Connection not found");
                await _dialogManager.ShowDialogAsync(dialog);
                return;
            }
            
            try
            {
                Server = new ChatServer(IPAddress.Parse(IpAddress), port);
                Server.Start();
            }
            catch (Exception e)
            {
                IsOn = false;
                this.OnPropertyChanged(null);
                var dialog = _viewModelFactory.CreateMessageBoxViewModel("Error", e.Message);
                await _dialogManager.ShowDialogAsync(dialog);
            }

            App.IsExit = false;
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
            this.OnPropertyChanged(null);
        }
    }
}