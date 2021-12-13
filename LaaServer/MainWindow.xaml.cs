using MahApps.Metro.Controls;
using System;
using LaaServer.Views;

namespace LaaServer
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            _mainFrame.Navigate(new RootPage());
        }
    }
}
