using Laa.Shared;
using LaaSender.Common.Network;
using System;
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
        }

        private async void ConnectButton_Clicked(object sender, EventArgs e)
        {
            await SecureStorage.SetAsync("lastIpAddress", ipaddressTxt.Text);

            Client = new ChatClient(ipaddressTxt.Text, 9091);
            Client.Connect();

            ConnectButton.Text = "Connecting...";

            if (Client.IsConnected)
            {
                ConnectButton.Text = "Connected";
                ConnectButton.TextColor = Color.Green;
            }
            else
            {
                ConnectButton.Text = "Connect";
                ConnectButton.TextColor = Color.Red;
            }

            //var ReceiverAudioAddress = new IPEndPoint(IPAddress.Parse(ipaddressTxt.Text), Util.ReceiverPort);
            //udpSender = new UdpClient();
            //udpSender.Connect(ReceiverAudioAddress);
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
