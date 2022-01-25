using System;
using Xamarin.Forms;
using InTheHand.Net.Sockets;

namespace LaaSender.Views
{
    public partial class BluetoothDevicesPage : ContentPage
    {
        IBluetoothRequestService BluetoothRequestService;
        IBluetoothService BluetoothService;

        public BluetoothDevicesPage()
        {
            InitializeComponent();

            BluetoothService = new BluetoothService();
            BluetoothRequestService = DependencyService.Get<IBluetoothRequestService>();

            DevicesList.Refreshing += (s, e) =>
            {
                Init();
                DevicesList.IsRefreshing = false;
            };

            Init();
        }

        async void Init()
        {
            if (BluetoothRequestService.IsEnabled())
            {
                DevicesList.ItemsSource = BluetoothService.PairedDevices();
            }
            else
            {
                bool res = await DisplayAlert("", "Your bluetooth will be turned on", "OK", "Cancel");
                if (res)
                {
                    EnableBluetooth();
                }
            }
        }

        private void SearchDevice(object sender, EventArgs e)
        {
            if (!BluetoothRequestService.IsEnabled())
            {
                EnableBluetooth();
                return;
            }

            DevicesList.ItemsSource = BluetoothService.PairedDevices();
        }

        async void EnableBluetooth()
        {
            bool result = await BluetoothRequestService.Enable();
            if (result)
            {
                App.Current.Dispatcher.BeginInvokeOnMainThread(() => 
                {
                    DevicesList.ItemsSource = BluetoothService.PairedDevices();
                });
            }
        }

        private async void ActionConnect(object sender, SelectedItemChangedEventArgs se)
        {
            try
            {
                BluetoothService.Connect((BluetoothDeviceInfo)DevicesList.SelectedItem);

                //if (!service.IsConnected())
                //{
                //    await DisplayAlert("ERROR", "Failed to connect :(", "OK");
                //    return;
                //}

                BluetoothPage page = new BluetoothPage(BluetoothService);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(page), false);
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERROR", ex.Message, "OK");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            App.ConfirmExit();
            return true;
        }
    }
}
