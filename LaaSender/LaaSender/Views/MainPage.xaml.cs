using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LaaSender.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void BluetoothButton_Clicked(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new MainPageBluetooth()), false);
            //App.Current.MainPage.Navigation.RemovePage(this);
        }

        private async void WifiButton_Clicked(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new WifiPage()), false);
            //App.Current.MainPage.Navigation.RemovePage(this);
        }
    }
}
