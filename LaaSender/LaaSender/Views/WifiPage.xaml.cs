using Acr.UserDialogs;
using Laa.Shared;
using LaaSender.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LaaSender.Views
{
    public partial class WifiPage : ContentPage
    {
        //ChatClient Client;
        EchoClient Client;
        bool MouseOn = false;

        public WifiPage()
        {
            InitializeComponent();

            SwitchViews();

            KeyboardEntry.TextChanged += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.NewTextValue))
                {
                    Client?.Send(e.NewTextValue + LaaConstants.FirstHash);
                    Client?.Send(e.NewTextValue + LaaConstants.SecondHash);
                }

                AllTxt.Text += e.NewTextValue;

                KeyboardEntry.Text = "";
            };

            ShowTextSwitch.IsToggled = true;

            ShowTextSwitch.Toggled += (s, e) =>
            {
                if (ShowTextSwitch.IsToggled)
                {
                    AllTxt.IsVisible = true;
                }
                else
                {
                    AllTxt.IsVisible = false;
                }
            };

            KeyboardEntry.OnBackspace += (s, e) =>
            {
                if (!string.IsNullOrEmpty(AllTxt.Text) && AllTxt.Text.Length > 0)
                {
                    AllTxt.Text = AllTxt.Text.Remove(AllTxt.Text.Length - 1);
                }

                Client?.Send("backspace" + LaaConstants.FirstBkHash);
                Client?.Send("backspace" + LaaConstants.SecondBkHash);

                //_service.Send("backspace");
                //Client?.Send("backspace");
            };

            Task<string> lastIpAddess = SecureStorage.GetAsync("lastIpAddress");
            lastIpAddess.Wait();

            if (!string.IsNullOrEmpty(lastIpAddess.Result))
            {
                ipaddressTxt.Text = lastIpAddess.Result;
            }

            ipaddressTxt.TextChanged += (s, e) =>
            {
                ipaddressTxt.TextColor = ConnectButton.TextColor;
            };

            //var tapGestureRecognizer = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
            //tapGestureRecognizer.Tapped += (s, e) =>
            //{
            //    Client?.Send(LaaConstants.Tapped1);
            //    Client?.Send(LaaConstants.Tapped2);
            //};
            //TouchPadBoxView.GestureRecognizers.Add(tapGestureRecognizer);

            TouchEffect touchEffect = new TouchEffect();
            touchEffect.TouchAction += OnTouchEffectAction;
            TouchPadBoxView.Effects.Add(touchEffect);
            //TouchPadGrid.Effects.Add(touchEffect);
        }

        private async void ConnectButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ipaddressTxt.Text))
            {
                await DisplayAlert("", "Ip address cannot be empty", "OK");
                return;
            }

            if (!IPAddress.TryParse(ipaddressTxt.Text, out _))
            {
                await DisplayAlert("", "Invalid ip address", "OK");
                return;
            }

            await SecureStorage.SetAsync("lastIpAddress", ipaddressTxt.Text);

            using (UserDialogs.Instance.Loading("Connecting...", null, null, true, MaskType.Gradient))
            {
                Client = new EchoClient(ipaddressTxt.Text, LaaConstants.WifiPort);

                CancellationTokenSource s_cts = new CancellationTokenSource();
                s_cts.CancelAfter(7500);

                await Task.Run(() => 
                {
                    Client.Connect();
                    while (!Client.IsConnected && !s_cts.IsCancellationRequested) {
                    }
                }, s_cts.Token);
            }

            if (Client.IsConnected)
            {
                ipaddressTxt.TextColor = Color.Green;

                var toastConfig = new ToastConfig("Connected");
                toastConfig.SetDuration(3000);

                UserDialogs.Instance.Toast(toastConfig);
                //await DisplayAlert("", "Connected", "OK");
            }
            else
            {
                Client.DisconnectAndStop();

                ipaddressTxt.TextColor = Color.Red;
                await DisplayAlert("", "Failed to connect :(", "OK");
            }
        }

        private void KeyboardOrMouseButton_Clicked(object sender, EventArgs e)
        {
            MouseOn = !MouseOn;
            SwitchViews();
        }

        void SwitchViews()
        {
            if (MouseOn)
            {
                KeyboardOrMouseButton.Text = "Show Keyboard";

                ShowTextStackPanel.IsVisible = false;
                AllTxt.IsVisible = false;
                KeyboardEntry.IsVisible = false;

                TouchPadFrame.IsVisible = true;
            }
            else
            {
                KeyboardOrMouseButton.Text = "Show Mouse";

                ShowTextStackPanel.IsVisible = true;
                AllTxt.IsVisible = true;
                KeyboardEntry.IsVisible = true;

                TouchPadFrame.IsVisible = false;

                KeyboardEntry.Focus();
            }
        }

        List<TouchPoint> TouchPoints = new List<TouchPoint>();

        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            var touchPoint = new TouchPoint
            {
                TouchActionType = args.Type,
                X = (int)args.Location.X,
                Y = (int)args.Location.Y,
                DateTimeTicks = DateTime.Now.Ticks
            };

            string json = JsonConvert.SerializeObject(touchPoint);

            switch (args.Type)
            {
                case TouchActionType.Entered:
                    break;
                case TouchActionType.Pressed:
                    break;
                case TouchActionType.Moved:
                    TouchPoints.Add(touchPoint);
                    //System.Console.WriteLine($"{touchPoint.X}, {touchPoint.Y}");
                    Client?.Send(json + LaaConstants.MouseLocationHash);
                    break;
                case TouchActionType.Released:
                    //Client?.Send(json + LaaConstants.MouseLocationHash);
                    TouchPoints.Clear();
                    break;
                case TouchActionType.Exited:
                    break;
                case TouchActionType.Cancelled:
                    break;
                default:
                    break;
            }
        }

        //protected override bool OnBackButtonPressed() => false;
        /*protected override bool OnBackButtonPressed() 
        {
            //See reference: https://stackoverflow.com/a/49282833
            //System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();

            //System.Diagnostics.Process.GetCurrentProcess().Kill();
            App.Exit();

            return false;
        }*/

        protected override bool OnBackButtonPressed()
        {
            App.ConfirmExit();
            return true;
        }

        private void LeftButton_Clicked(object sender, EventArgs e)
        {
            Client?.Send(LaaConstants.Tapped1);
            Client?.Send(LaaConstants.Tapped2);
        }

        private void RightButton_Clicked(object sender, EventArgs e)
        {
            Client?.Send(LaaConstants.Tapped1);
            Client?.Send(LaaConstants.Tapped2);
        }
    }
}