using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Linq;
using Laa.Shared;
using Newtonsoft.Json;

namespace LaaSender.Views
{
    public partial class BluetoothPage : ContentPage
    {
        bool MouseOn = false;
        private IBluetoothService _service;

        public BluetoothPage(IBluetoothService service)
        {
            _service = service;

            InitializeComponent();

            SwitchViews();

            KeyboardEntry.TextChanged += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.NewTextValue))
                {
                    _service.Send(e.NewTextValue + LaaConstants.FirstHash);
                    _service.Send(e.NewTextValue + LaaConstants.SecondHash);
                }

                AllTxt.Text += e.NewTextValue;

                KeyboardEntry.Text = "";
            };

            //KeyboardEntry.IsSpellCheckEnabled = true;

            ShowTextSwitch.IsToggled = true;

            ShowTextSwitch.Toggled += (s, e) =>
            {
                if (ShowTextSwitch.IsToggled)
                {
                    AllTxt.IsVisible = true;
                }
                else
                {
                    AllTxt.IsVisible = false;
                }
            };

            KeyboardEntry.OnBackspace += (s, e) =>
            {
                if (!string.IsNullOrEmpty(AllTxt.Text) && AllTxt.Text.Length > 0)
                {
                    AllTxt.Text = AllTxt.Text.Remove(AllTxt.Text.Length - 1);
                }

                _service.Send("backspace" + LaaConstants.FirstBkHash);
                _service.Send("backspace" + LaaConstants.SecondBkHash);
            };

            TouchEffect touchEffect = new TouchEffect();
            touchEffect.TouchAction += OnTouchEffectAction;
            TouchPadBoxView.Effects.Add(touchEffect);
        }

        private void KeyboardOrMouseButton_Clicked(object sender, EventArgs e)
        {
            MouseOn = !MouseOn;
            SwitchViews();
        }

        void SwitchViews()
        {
            if (MouseOn)
            {
                KeyboardOrMouseButton.Text = "Show Keyboard";

                ShowTextStackPanel.IsVisible = false;
                AllTxt.IsVisible = false;
                KeyboardEntry.IsVisible = false;

                TouchPadFrame.IsVisible = true;
            }
            else
            {
                KeyboardOrMouseButton.Text = "Show Mouse";

                ShowTextStackPanel.IsVisible = true;
                AllTxt.IsVisible = true;
                KeyboardEntry.IsVisible = true;

                TouchPadFrame.IsVisible = false;

                KeyboardEntry.Focus();
            }
        }

        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            var pt = new TouchPoint
            {
                TouchActionType = args.Type,
                X = (int)args.Location.X,
                Y = (int)args.Location.Y
            };

            string json = JsonConvert.SerializeObject(pt);

            switch (args.Type)
            {
                case TouchActionType.Entered:
                    break;
                case TouchActionType.Pressed:
                    break;
                case TouchActionType.Moved:
                    if (_service.IsConnected())
                    {
                        _service.Send(json + LaaConstants.MouseLocationHash);
                    }
                    break;
                case TouchActionType.Released:
                    //Don't use here: if (_service.IsConnected())
                    _service.Send(json + LaaConstants.MouseLocationHash);
                    break;
                case TouchActionType.Exited:
                    break;
                case TouchActionType.Cancelled:
                    break;
                default:
                    break;
            }
        }

        private void LeftButton_Clicked(object sender, EventArgs e)
        {
            if (_service.IsConnected())
            {
                _service.Send(LaaConstants.LeftClicked1);
                _service.Send(LaaConstants.LeftClicked2);
            }
        }

        private void RightButton_Clicked(object sender, EventArgs e)
        {
            if (_service.IsConnected())
            {
                _service.Send(LaaConstants.RightClicked1);
                _service.Send(LaaConstants.RightClicked2);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override async void OnAppearing()
        {
            //RefreshUI();
            //await DisconnectIfConnectedAsync();

            base.OnAppearing();
        }

        private async Task DisconnectIfConnectedAsync()
        {
            if (_service != null)
            {
                try
                {
                    _service.Disconnect();
                }
                catch (Exception exception)
                {
                    await DisplayAlert("Error", exception.Message, "Close");
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            App.ConfirmExit();
            return true;
        }
    }
}