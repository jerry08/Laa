using LaaServer.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LaaServer.Views
{
    public partial class RootPage : Page
    {
        public RootPage()
        {
            InitializeComponent();
        }

        private void WifiButton_Click(object sender, RoutedEventArgs e)
        {
            (App.Current.MainWindow as RootView)._mainFrame.NavigationService.Navigate(new WifiPage());
        }

        private void BthButton_Click(object sender, RoutedEventArgs e)
        {
            (App.Current.MainWindow as RootView)._mainFrame.NavigationService.Navigate(new BluetoothPage());
        }
        
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            (App.Current.MainWindow.DataContext as RootViewModel).ShowSettings();
        }
    }
}
