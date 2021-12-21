using LaaSender.Common.Network;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using System.Linq;
using Laa.Shared;
using Newtonsoft.Json;

namespace LaaSender.Views
{
    public partial class BluetoothPage : ContentPage
    {
        private IBluetoothService _service;

        public BluetoothPage(IBluetoothService service)
        {
            _service = service;

            InitializeComponent();

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

            AllTxt.IsVisible = false;
            ShowTextSwitch.IsToggled = false;

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

        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            var pt = new TouchPoint
            {
                TouchActionType = (int)args.Type,
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

        protected override bool OnBackButtonPressed()
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return false;
        }
    }
}
