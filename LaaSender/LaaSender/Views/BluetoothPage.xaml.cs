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

namespace LaaSender.Views
{
    public partial class BluetoothPage : ContentPage
    {
        private IBluetoothService _service;

        public BluetoothPage(IBluetoothService service)
        {
            _service = service;

            InitializeComponent();

            //var pages = App.Current.MainPage.Navigation
            //    .NavigationStack.ToList();
            //
            //for (int i = 0; i < pages.Count; i++)
            //{
            //    App.Current.MainPage.Navigation.RemovePage(pages[i]);
            //}

            //KeyboardEntry.IsVisible = false;
            //ShowKeyboardButton.Text = "Show Keyboard";
            //
            //KeyboardEntry.Unfocused += (s, e) =>
            //{
            //    HideKeyboard();
            //};

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

                //_service.Send("backspace");
                //Client?.Send("backspace");
            };
        }

        /*private void ShowKeyboardButton_Clicked(object sender, EventArgs e)
        {
            if (KeyboardEntry.IsFocused)
            {
                HideKeyboard();
            }
            else
            {
                ShowKeyboard();
            }
        }

        void HideKeyboard()
        {
            ShowKeyboardButton.Text = "Show Keyboard";
            KeyboardEntry.Unfocus();
            KeyboardEntry.IsVisible = false;
        }

        void ShowKeyboard()
        {
            ShowKeyboardButton.Text = "Hide Keyboard";
            KeyboardEntry.IsVisible = true;
            KeyboardEntry.Focus();
        }*/
    }
}
