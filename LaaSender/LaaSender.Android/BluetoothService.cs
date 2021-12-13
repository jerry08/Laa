using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
//using BluetoothSample.Droid.PlatformSpecifics;
//using BluetoothSample.Services.Interfaces;
using Java.Util;
using LaaSender.Droid;
using Xamarin.Forms;
using Debug = System.Diagnostics.Debug;

[assembly: Dependency(typeof(BluetoothService))]
namespace LaaSender.Droid
{
    public class BluetoothService : IBluetoothService
    {
        //private CancellationTokenSource _cancellationToken { get; }

        public string MessageToSend { get; set; }

        public BluetoothService()
        {
            //_cancellationToken = new CancellationTokenSource();
        }

        public void Connect(string name)
        {
            Task.Run(async () => await ConnectDevice(name));
        }

        public bool IsConnected()
        {
            //return bthSocket != null && bthSocket.IsConnected;
            return true;
        }

        /*BluetoothSocket bthSocket = null;
        private async Task ConnectDevice(string name)
        {
            BluetoothDevice device = null;
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            bthSocket = null;

            while (_cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    Thread.Sleep(250);

                    adapter = BluetoothAdapter.DefaultAdapter;

                    if (adapter == null)
                        Debug.Write("No bluetooth adapter found!");
                    else
                        Debug.Write("Adapter found!");

                    if (!adapter.IsEnabled)
                        Debug.Write("Bluetooth adapter is not enabled.");
                    else
                        Debug.Write("Adapter found!");

                    Debug.Write("Try to connect to " + name);

                    foreach (var bondedDevice in adapter.BondedDevices)
                    {
                        Debug.Write("Paired devices found: " + bondedDevice.Name.ToUpper());

                        if (bondedDevice.Name.ToUpper().IndexOf(name.ToUpper()) >= 0)
                        {
                            Debug.Write("Found " + bondedDevice.Name + ". Try to connect with it!");
                            device = bondedDevice;

                            break;
                        }
                    }

                    if (device == null)
                        Debug.Write("Named device not found.");
                    else
                    {
                        UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");

                        bthSocket = device.CreateRfcommSocketToServiceRecord(uuid);

                        if (bthSocket != null)
                        {
                            adapter.CancelDiscovery();

                            await bthSocket.ConnectAsync();

                            if (bthSocket.IsConnected)
                            {
                                Debug.Write("Connected");

                                while (_cancellationToken.IsCancellationRequested == false)
                                {
                                    if (MessageToSend != null)
                                    {
                                        var chars = MessageToSend.ToCharArray();
                                        var bytes = new List<byte>();

                                        foreach (var character in chars)
                                        {
                                            bytes.Add((byte)character);
                                        }

                                        await bthSocket.OutputStream.WriteAsync(bytes.ToArray(), 0, bytes.Count);

                                        MessageToSend = null;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write(ex);
                    Debug.Write(ex.Message);
                }
                finally
                {
                    if (bthSocket != null)
                        bthSocket.Close();

                    device = null;
                    adapter = null;
                }
            }
        }*/

        /*CancellationTokenSource _cancellationToken;
        private async Task ConnectDevice(string name)
        {
            BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            BluetoothDevice bluetoothDevice = null;

            foreach (var bondedDevice in bluetoothAdapter.BondedDevices)
            {
                Debug.Write("Paired devices found: " + bondedDevice.Name.ToUpper());

                if (bondedDevice.Name.ToUpper().IndexOf(name.ToUpper()) >= 0)
                {
                    Debug.Write("Found " + bondedDevice.Name + ". Try to connect with it!");
                    bluetoothDevice = bondedDevice;

                    break;
                }
            }

            //BluetoothDevice bluetoothDevice = bluetoothAdapter.GetRemoteDevice ("00:26:5E:DE:7D:FC");
            BluetoothSocket bluetoothSocket = null;
            System.IO.Stream outStream = null;

            IntPtr createRfcommSocket = JNIEnv.GetMethodID(bluetoothDevice.Class.Handle, "createRfcommSocket", "(I)Landroid/bluetooth/BluetoothSocket;");
            IntPtr socket = JNIEnv.CallObjectMethod(bluetoothDevice.Handle, createRfcommSocket, new Android.Runtime.JValue(1));
            bluetoothSocket = Java.Lang.Object.GetObject<BluetoothSocket>(socket, JniHandleOwnership.TransferLocalRef);

            Java.Util.UUID serialUUID = Java.Util.UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");

            bluetoothSocket = bluetoothDevice.CreateRfcommSocketToServiceRecord(serialUUID);   
            await bluetoothSocket.ConnectAsync();
            //outStream = bluetoothSocket.OutputStream;

            string data = "testing";

            bluetoothSocket.OutputStream.Write(Encoding.ASCII.GetBytes(data), 0, data.Length);
            bluetoothSocket.Close();

            return;

            BluetoothDevice device = null;
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            BluetoothSocket bthSocket = null;
            BluetoothServerSocket bthServerSocket = null;

            UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");
            bthServerSocket = adapter.ListenUsingRfcommWithServiceRecord("TLCI Barcode Scanner", uuid);

            _cancellationToken = new CancellationTokenSource();

            while (_cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    Thread.Sleep(250);

                    adapter = BluetoothAdapter.DefaultAdapter;

                    if (adapter == null)
                        Debug.Write("No bluetooth adapter found!");
                    else
                        Debug.Write("Adapter found!");

                    if (!adapter.IsEnabled)
                        Debug.Write("Bluetooth adapter is not enabled.");
                    else
                        Debug.Write("Adapter found!");

                    Debug.Write("Try to connect to " + name);

                    foreach (var bondedDevice in adapter.BondedDevices)
                    {
                        Debug.Write("Paired devices found: " + bondedDevice.Name.ToUpper());

                        if (bondedDevice.Name.ToUpper().IndexOf(name.ToUpper()) >= 0)
                        {
                            Debug.Write("Found " + bondedDevice.Name + ". Try to connect with it!");
                            device = bondedDevice;
                            Debug.Write(bondedDevice.Type.ToString());
                            break;
                        }
                    }

                    if (device == null)
                        Debug.Write("Named device not found.");
                    else
                    {
                        bthSocket = bthServerSocket.Accept();

                        adapter.CancelDiscovery();

                        if (bthSocket != null)
                        {
                            Debug.Write("Connected");

                            if (bthSocket.IsConnected)
                            {
                                var mReader = new InputStreamReader(bthSocket.InputStream);
                                var buffer = new BufferedReader(mReader);

                                while (_cancellationToken.IsCancellationRequested == false)
                                {
                                    if (MessageToSend != null)
                                    {
                                        var chars = MessageToSend.ToCharArray();
                                        var bytes = new List<byte>();

                                        foreach (var character in chars)
                                        {
                                            bytes.Add((byte)character);
                                        }

                                        await bthSocket.OutputStream.WriteAsync(bytes.ToArray(), 0, bytes.Count);

                                        MessageToSend = null;
                                    }
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write(ex);
                    Debug.Write(ex.Message);
                }
                finally
                {
                    if (bthSocket != null)
                        bthSocket.Close();

                    device = null;
                    adapter = null;
                }
            }
        }*/

        CancellationTokenSource _cancellationToken;
        private async Task ConnectDevice(string name)
        {
            /*BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            BluetoothDevice bluetoothDevice = null;

            foreach (var bondedDevice in bluetoothAdapter.BondedDevices)
            {
                Debug.Write("Paired devices found: " + bondedDevice.Name.ToUpper());

                if (bondedDevice.Name.ToUpper().IndexOf(name.ToUpper()) >= 0)
                {
                    Debug.Write("Found " + bondedDevice.Name + ". Try to connect with it!");
                    bluetoothDevice = bondedDevice;

                    break;
                }
            }

            //BluetoothDevice bluetoothDevice = bluetoothAdapter.GetRemoteDevice ("00:26:5E:DE:7D:FC");
            BluetoothSocket bluetoothSocket = null;
            System.IO.Stream outStream = null;

            IntPtr createRfcommSocket = JNIEnv.GetMethodID(bluetoothDevice.Class.Handle, "createRfcommSocket", "(I)Landroid/bluetooth/BluetoothSocket;");
            IntPtr socket = JNIEnv.CallObjectMethod(bluetoothDevice.Handle, createRfcommSocket, new Android.Runtime.JValue(1));
            bluetoothSocket = Java.Lang.Object.GetObject<BluetoothSocket>(socket, JniHandleOwnership.TransferLocalRef);

            Java.Util.UUID serialUUID = Java.Util.UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");

            bluetoothSocket = bluetoothDevice.CreateRfcommSocketToServiceRecord(serialUUID);
            await bluetoothSocket.ConnectAsync();
            //outStream = bluetoothSocket.OutputStream;

            string data = "testing";

            bluetoothSocket.OutputStream.Write(Encoding.ASCII.GetBytes(data), 0, data.Length);
            bluetoothSocket.Close();

            return;*/

            BluetoothDevice device = null;
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            BluetoothSocket bthSocket = null;
            //BluetoothServerSocket bthServerSocket = null;

            UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");
            //bthServerSocket = adapter.ListenUsingRfcommWithServiceRecord("TLCI Barcode Scanner", uuid);

            _cancellationToken = new CancellationTokenSource();

            while (_cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    Thread.Sleep(250);

                    adapter = BluetoothAdapter.DefaultAdapter;

                    if (adapter == null)
                        Debug.Write("No bluetooth adapter found!");
                    else
                        Debug.Write("Adapter found!");

                    if (!adapter.IsEnabled)
                        Debug.Write("Bluetooth adapter is not enabled.");
                    else
                        Debug.Write("Adapter found!");

                    Debug.Write("Try to connect to " + name);

                    foreach (var bondedDevice in adapter.BondedDevices)
                    {
                        Debug.Write("Paired devices found: " + bondedDevice.Name.ToUpper());

                        if (bondedDevice.Name.ToUpper().IndexOf(name.ToUpper()) >= 0)
                        {
                            Debug.Write("Found " + bondedDevice.Name + ". Try to connect with it!");
                            device = bondedDevice;
                            Debug.Write(bondedDevice.Type.ToString());
                            break;
                        }
                    }

                    if (device == null)
                        Debug.Write("Named device not found.");
                    else
                    {
                        //bthSocket = bthServerSocket.Accept();
                        //bthSocket = device.CreateRfcommSocketToServiceRecord(uuid);
                        bthSocket = device.CreateInsecureRfcommSocketToServiceRecord(uuid);

                        adapter.CancelDiscovery();

                        //var context = MainActivity.Instance;
                        //var test = bthSocket.RemoteDevice.ConnectGatt(context, false, null);
                        //test.RequestConnectionPriority(GattConnectionPriority.High);
                        //bthSocket.RemoteDevice.ConnectGatt(context, true, null).RequestConnectionPriority(GattConnectionPriority.High);

                        if (bthSocket != null)
                        {
                            await bthSocket.ConnectAsync();

                            Debug.Write("Connected");

                            if (bthSocket.IsConnected)
                            {
                                //var mReader = new InputStreamReader(bthSocket.InputStream);
                                //var buffer = new BufferedReader(mReader);

                                while (_cancellationToken.IsCancellationRequested == false)
                                {
                                    //if (!string.IsNullOrEmpty(MessageToSend))
                                    if (Messages.Count > 0 && !string.IsNullOrEmpty(Messages[0]))
                                    {
                                        var msg = Messages[0];

                                        byte[] msgBytes = Encoding.UTF8.GetBytes(msg);

                                        //await bthSocket.OutputStream.WriteAsync(Encoding.ASCII.GetBytes(MessageToSend), 0, MessageToSend.Length);
                                        //bthSocket.OutputStream.Write(Encoding.ASCII.GetBytes(MessageToSend), 0, MessageToSend.Length);
                                        await bthSocket.OutputStream.WriteAsync(msgBytes, 0, msgBytes.Length);
                                        //bthSocket.Close();
                                        bthSocket.OutputStream.Close();

                                        bthSocket = device.CreateInsecureRfcommSocketToServiceRecord(uuid);
                                        await bthSocket.ConnectAsync();

                                        //bthSocket.OutputStream.Close();
                                        //bthSocket.OutputStream.Flush();

                                        //bthSocket.OutputStream.Close();
                                        //Device.InvokeOnMainThreadAsync(SomeAsyncMethod);

                                        //await bthSocket.ConnectAsync();

                                        //var chars = MessageToSend.ToCharArray();
                                        //var bytes = new List<byte>();
                                        //
                                        //foreach (var character in chars)
                                        //{
                                        //    bytes.Add((byte)character);
                                        //}
                                        //
                                        //await bthSocket.OutputStream.WriteAsync(bytes.ToArray(), 0, bytes.Count);

                                        Messages.RemoveAt(0);

                                        MessageToSend = null;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write(ex);
                    Debug.Write(ex.Message);
                }
                finally
                {
                    if (bthSocket != null)
                        bthSocket.Close();

                    device = null;
                    adapter = null;
                }
            }
        }

        public void Disconnect()
        {
            if (_cancellationToken != null)
            {
                _cancellationToken.Cancel();
            }
        }

        public List<string> PairedDevices()
        {
            if (!BluetoothAdapter.DefaultAdapter.IsEnabled)
                BluetoothAdapter.DefaultAdapter.Enable();

            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            List<string> devices = new List<string>();

            foreach (var bondedDevices in adapter.BondedDevices)
                devices.Add(bondedDevices.Name);

            return devices;
        }

        List<string> Messages = new List<string>();

        public void Send(string message)
        {
            //if (MessageToSend == null)
            //    MessageToSend = message;

            if (!string.IsNullOrEmpty(message))
            {
                MessageToSend = message;
                Messages.Add(message);
            }
        }

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