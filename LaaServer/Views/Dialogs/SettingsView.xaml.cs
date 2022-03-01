using System.Windows;

namespace LaaServer.Views.Dialogs
{
    public partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void DarkModeToggleButton_Checked(object sender, RoutedEventArgs args) =>
            App.SetDarkTheme();

        private void DarkModeToggleButton_Unchecked(object sender, RoutedEventArgs args) =>
            App.SetLightTheme();

        private void StartAppOnBootToggleButton_Checked(object sender, RoutedEventArgs e) =>
            App.SetStartup(true);

        private void StartAppOnBootToggleButton_Unchecked(object sender, RoutedEventArgs e) =>
            App.SetStartup(false);
    }
}