using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LaaSender
{
    public class OTPCustomEntry : Entry
    {
        public delegate void BackspaceEventHandler(object sender, EventArgs e);

        public event BackspaceEventHandler OnBackspace;

        public OTPCustomEntry()
        {

        }

        public void OnBackspacePressed()
        {
            if (OnBackspace != null)
            {
                OnBackspace(null, null);
            }
        }
    }
}
