using Acr.UserDialogs;
using Laa.Shared;
using LaaSender.Common.Network;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LaaSender.Views
{
    public partial class WifiPage : ContentPage
    {
        ChatClient Client;

        public WifiPage()
        {
            InitializeComponent();

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

            AllTxt.IsVisible = false;
            ShowTextSwitch.IsToggled = false;

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

            TouchEffect touchEffect = new TouchEffect();
            touchEffect.TouchAction += OnTouchEffectAction;
            TouchPadBoxView.Effects.Add(touchEffect);
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

            using (UserDialogs.Instance.Loading("Connecting...", null, null, true, MaskType.Black))
            {
                Client = new ChatClient(ipaddressTxt.Text, 9091);

                await Task.Run(() => 
                {
                    Client.Connect();
                });
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
                ipaddressTxt.TextColor = Color.Red;
                await DisplayAlert("", "Failed to connect :(", "OK");
            }
        }

        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            var pt = new TouchPoint
            {
                TouchActionType = args.Type,
                X = (int)args.Location.X,
                Y = (int)args.Location.Y
            };

            string json = JsonConvert.SerializeObject(pt);

            switch (args.Type)
            {
                case TouchActionType.Entered:
                    break;
                case TouchActionType.Pressed:
                    break;
                case TouchActionType.Moved:
                    Client?.Send(json + LaaConstants.MouseLocationHash);
                    break;
                case TouchActionType.Released:
                    Client?.Send(json + LaaConstants.MouseLocationHash);
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
        protected override bool OnBackButtonPressed() 
        {
            //See reference: https://stackoverflow.com/a/49282833
            //System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return false;
        }
    }
}
