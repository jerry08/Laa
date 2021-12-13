using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Views;
using Android.Content;
using LaaSender.Droid;
using LaaSender;

[assembly: ExportRenderer(typeof(OTPCustomEntry), typeof(OTPCustomEntryRenderer_android))]
namespace LaaSender.Droid
{
    public class OTPCustomEntryRenderer_android : EntryRenderer
    {
        public OTPCustomEntryRenderer_android(Context context) : base(context)
        {
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (e.Action == KeyEventActions.Down)
            {
                if (e.KeyCode == Keycode.Del)
                {
                    if (string.IsNullOrWhiteSpace(Control.Text))
                    {
                        var entry = (OTPCustomEntry)Element;
                        entry.OnBackspacePressed();
                    }
                }
            }
            return base.DispatchKeyEvent(e);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
        }
    }
}