using APES.UI.XF;
using LaaSender.ViewModels;
using LaaSender.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LaaSender
{
    public partial class App : Application
    {
        public static INavigation Navigation { get; internal set; }

        public App()
        {
            InitializeComponent();

            //MainPage = new MainPage();
            //MainPage = new NavigationPage(new MainPageBluetooth());
            MainPage = new NavigationPage(new MainPage());

            Sharpnado.MaterialFrame.Initializer.Initialize(loggerEnable: false, false);

            SettingsViewModel settingsViewModel = new SettingsViewModel();
            settingsViewModel.SetMaterialFrameStyle();
            settingsViewModel.SetAppTheme();

            ContextMenuContainer.Init();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
