using APES.UI.XF;
using Laa.Shared;
using LaaSender.ViewModels;
using LaaSender.Views;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
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

            //Cache first
            JsonConvert.SerializeObject(new TouchPoint());
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

        public static void ConfirmExit() 
        {
            //See reference: https://stackoverflow.com/a/49282833
            //System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();

            //System.Diagnostics.Process.GetCurrentProcess().Kill();

            //Task<bool> action = Current.MainPage.DisplayAlert("Confirmation", "Are you sure you want to exit the application?", "Yes", "No");
            //action.ContinueWith(task =>
            //{
            //    if (task.Result)
            //    {
            //        System.Diagnostics.Process.GetCurrentProcess().Kill();
            //    }
            //});

            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await Current.MainPage.DisplayAlert("Confirmation", "Do you really want to exit?", "Yes", "Cancel");

                if (result)
                {
                    //System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow(); // Or anything else
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            });
        }
    }
}
