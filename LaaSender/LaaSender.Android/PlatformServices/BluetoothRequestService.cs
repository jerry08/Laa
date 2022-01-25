using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using LaaSender.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(BluetoothRequestService))]
namespace LaaSender.Droid
{
    public class BluetoothRequestService : IBluetoothRequestService
    {
        public bool IsEnabled()
        {
            return BluetoothAdapter.DefaultAdapter.IsEnabled;
        }

        public Task<bool> Enable()
        {
            return Task.Run(() =>
            {
                if (!BluetoothAdapter.DefaultAdapter.IsEnabled)
                {
                    BluetoothAdapter.DefaultAdapter.Enable();
                }

                while (!BluetoothAdapter.DefaultAdapter.IsEnabled)
                {
                }
            }).ContinueWith((s) =>
            {
                return true;
            });
        }
    }
}