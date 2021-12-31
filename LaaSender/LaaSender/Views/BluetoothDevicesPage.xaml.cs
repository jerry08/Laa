using System;
using Xamarin.Forms;

namespace LaaSender.Views
{
    public partial class BluetoothDevicesPage : ContentPage
    {
        IBluetoothService service;

        public BluetoothDevicesPage()
        {
            InitializeComponent();

            service = DependencyService.Get<IBluetoothService>();

            DevicesList.Refreshing += (s, e) =>
            {
                Init();
                DevicesList.IsRefreshing = false;
            };

            Init();
        }

        async void Init()
        {
            if (service.IsEnabled())
            {
                DevicesList.ItemsSource = service.PairedDevices();
            }
            else
            {
                bool res = await DisplayAlert("", "Your bluetooth will be turned on", "OK", "Cancel");
                if (res)
                {
                    if (!service.IsEnabled())
                    {
                        EnableBluetooth();
                    }
                }
            }
        }

        private void searchDevice(object sender, EventArgs e)
        {
            if (!service.IsEnabled())
            {
                EnableBluetooth();
                return;
            }

            DevicesList.ItemsSource = service.PairedDevices();
        }

        async void EnableBluetooth()
        {
            bool result = await service.Enable();
            if (result)
            {
                App.Current.Dispatcher.BeginInvokeOnMainThread(() => 
                {
                    DevicesList.ItemsSource = service.PairedDevices();
                });
            }
        }

        private async void ActionConnect(object sender, SelectedItemChangedEventArgs se)
        {
            try
            {
                service.Connect(DevicesList.SelectedItem?.ToString());

                //if (!service.IsConnected())
                //{
                //    await DisplayAlert("ERROR", "Failed to connect :(", "OK");
                //    return;
                //}

                BluetoothPage page = new BluetoothPage(service);
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
