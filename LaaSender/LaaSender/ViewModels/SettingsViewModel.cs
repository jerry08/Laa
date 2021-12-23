using Xamarin.Forms;
using Xamarin.Essentials;
using System.Windows.Input;
using Sharpnado.MaterialFrame;

namespace LaaSender.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public string AppVersion
        {
            get 
            {
                return VersionTracking.CurrentVersion;
            }
        }
        
        public bool LightTheme
        {
            get => Preferences.Get(nameof(LightTheme), false);
            set
            {
                Preferences.Set(nameof(LightTheme), value);
                OnPropertyChanged(nameof(LightTheme));
                SetAppTheme();
                SetMaterialFrameStyle();
            }
        }

        public bool DarkTheme
        {
            get => Preferences.Get(nameof(DarkTheme), false);
            set
            {
                Preferences.Set(nameof(DarkTheme), value);
                OnPropertyChanged(nameof(DarkTheme));
                SetAppTheme();
                SetMaterialFrameStyle();
            }
        }

        public bool DefaultTheme
        {
            get => Preferences.Get(nameof(DefaultTheme), true);
            set
            {
                Preferences.Set(nameof(DefaultTheme), value);
                OnPropertyChanged(nameof(DefaultTheme));
                SetAppTheme();
                SetMaterialFrameStyle();
            }
        }

        public bool IsAcrylic
        {
            get => Preferences.Get(nameof(IsAcrylic), Device.RuntimePlatform == Device.Android ? false : true);
            set
            {
                Preferences.Set(nameof(IsAcrylic), value);
                OnPropertyChanged(nameof(IsAcrylic));
                SetMaterialFrameStyle();
            }
        }

        public ICommand GithubCommand { get; set; }
        public ICommand DoneCommand { get; set; }

        public SettingsViewModel()
        {
            DoneCommand = new Command(Done);
            GithubCommand = new Command(Github);
            SetAppTheme();
        }

        public void SetAppTheme()
        {
            App.Current.Resources["BlurTheme"] = MaterialFrame.Theme.AcrylicBlur;

            if (DefaultTheme)
                //Application.Current.UserAppTheme = OSAppTheme.Unspecified;
                Application.Current.UserAppTheme = OSAppTheme.Dark;
            else if (DarkTheme)
                Application.Current.UserAppTheme = OSAppTheme.Dark;
            else if (LightTheme)
                Application.Current.UserAppTheme = OSAppTheme.Light;
            else
                Application.Current.UserAppTheme = OSAppTheme.Unspecified;
        }

        public void SetMaterialFrameStyle()
        {
            var mf = new MaterialFrame();

            if (IsAcrylic == true)
            {
                App.Current.Resources["BlurTheme"] = MaterialFrame.Theme.AcrylicBlur;
            }
            else if (IsAcrylic == false && mf.MaterialBlurStyle == MaterialFrame.BlurStyle.Dark)
            {
                App.Current.Resources["BlurTheme"] = MaterialFrame.Theme.Dark;
            }
            else if (IsAcrylic == false && mf.MaterialBlurStyle == MaterialFrame.BlurStyle.ExtraLight)
            {
                App.Current.Resources["BlurTheme"] = MaterialFrame.Theme.Acrylic;
            }
        }

        async void Github()
        {
            await Browser.OpenAsync("https://github.com/jerry08/Laa");
        }
        
        async void Done()
        {
            await App.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}