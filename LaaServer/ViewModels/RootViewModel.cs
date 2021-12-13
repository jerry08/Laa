using Gress;
using LaaServer.Common.Network;
using System.Windows.Input;
using LaaServer.Views;

namespace LaaServer.ViewModels
{
    public class RootViewModel : PropertyChangedBase
    {
        #region Commands
        private ICommand _wifiButtonCommand;
        public ICommand WifiButtonCommand => _wifiButtonCommand ??= new CommandHandler((s) => WifiButton_Click(), () => CanExecute);

        private ICommand _bthButtonCommand;
        public ICommand BthButtonCommand => _bthButtonCommand ??= new CommandHandler((s) => BthButton_Click(), () => CanExecute);

        public bool CanExecute => true;
        #endregion

        public RootViewModel()
        {
            
        }

        private void WifiButton_Click()
        {
            (App.Current.MainWindow as MainWindow)._mainFrame
                .NavigationService.Navigate(new WifiPage());
        }

        private void BthButton_Click()
        {
            (App.Current.MainWindow as MainWindow)._mainFrame
                .NavigationService.Navigate(new BluetoothPage());
        }
    }
}