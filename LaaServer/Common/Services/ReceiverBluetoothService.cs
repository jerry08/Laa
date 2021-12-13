using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using InTheHand.Net.Sockets;
using System.Linq;
using InTheHand.Net.Bluetooth;

namespace LaaServer.Common.Services
{
    /// <summary>
    /// Define the receiver Bluetooth service.
    /// </summary>
    public class ReceiverBluetoothService : IDisposable, IReceiverBluetoothService
    {
        private readonly Guid _serviceClassId;
        private Action<string> _responseAction;
        private BluetoothListener _listener;
        private CancellationTokenSource _cancelSource;
        //private bool _wasStarted;
        //private string _status;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverBluetoothService" /> class.
        /// </summary>
        public ReceiverBluetoothService()
        {
            BluetoothClient client = new BluetoothClient();
            //List<string> items = new List<string>();
            //var devices = client.DiscoverDevices().ToList();
            var pairedDevices = client.PairedDevices.ToList();
            //BluetoothDeviceInfo[] devices = client.DiscoverDevicesInRange();
            //foreach (BluetoothDeviceInfo d in devices)
            //{
            //    items.Add(d.DeviceName);
            //}

            //var g = pairedDevices[2].InstalledServices.Where(x => x.ToString() == "00001101-0000-1000-8000-00805f9b34fb");

            //var test = InTheHand.Net.Bluetooth.BluetoothService.;
            _serviceClassId = new Guid("00001101-0000-1000-8000-00805f9b34fb");


            //var pick = new BluetoothDevicePicker();
            //pick.PickSingleDeviceAsync().ContinueWith(task =>
            //{
            //    var dev = task.Result;
            //    //Debug.Log(dev.DeviceName);
            //});


            /*var devices = client.DiscoverDevices().ToList();
            var device = devices.FirstOrDefault();

            device = pairedDevices[1];

            if (device != null)
            {
                Console.Out.WriteLine("Found Device!");
                Console.Out.WriteLine("    Name: " + device.DeviceName);
                Console.Out.WriteLine("    Address: " + device.DeviceAddress);
                Console.Out.WriteLine("    Authenticated: " + device.Authenticated);
                Console.Out.WriteLine("    Connected: " + device.Connected);
                Console.Out.WriteLine("    HID: " + device.InstalledServices.Contains(BluetoothService.HumanInterfaceDevice));

                try
                {
                    Console.Out.WriteLine("Enabling Service...");
                    device.SetServiceState(BluetoothService.HumanInterfaceDevice, true);
                    Console.Out.WriteLine("Connecting...");
                    client.Connect(device.DeviceAddress, BluetoothService.HumanInterfaceDevice); // Program.cs:line 36
                    Console.Out.WriteLine("Connected!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to connect.");
                    Console.Out.WriteLine(e);
                }

                var stream = client.GetStream();

                // ...

                Console.ReadLine();
            }*/
        }

        /// <summary>
        /// Gets or sets a value indicating whether was started.
        /// </summary>
        /// <value>
        /// The was started.
        /// </value>
        //public bool WasStarted
        //{
        //    get { return _wasStarted; }
        //    set { Set(() => WasStarted, ref _wasStarted, value); }
        //}

        public bool WasStarted { get; set; }

        /// <summary>
        /// Starts the listening from Senders.
        /// </summary>
        /// <param name="reportAction">
        /// The report Action.
        /// </param>
        public void Start(Action<string> reportAction)
        {
            WasStarted = true;
            _responseAction = reportAction;
            if (_cancelSource != null && _listener != null)
            {
                Dispose(true);
            }
            _listener = new BluetoothListener(_serviceClassId)
            {
                ServiceName = "MyService"
            };
            _listener.Start();

            _cancelSource = new CancellationTokenSource();

            Task.Run(() => Listener(_cancelSource));
        }

        /// <summary>
        /// Stops the listening from Senders.
        /// </summary>
        public void Stop()
        {
            WasStarted = false;
            _cancelSource.Cancel();
        }

        /// <summary>
        /// Listeners the accept bluetooth client.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        private void Listener(CancellationTokenSource token)
        {
            try
            {
                while (true)
                {
                    using (var client = _listener.AcceptBluetoothClient())
                    {
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        //using (var streamReader = new StreamReader(client.GetStream()))
                        using (var streamReader = new StreamReader(client.GetStream(), System.Text.Encoding.UTF8))
                        {
                            try
                            {
                                string content = streamReader.ReadToEnd();
                                if (!string.IsNullOrEmpty(content))
                                {
                                    _responseAction(content);
                                }
                            }
                            catch (IOException e)
                            {
                                client.Close();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // todo handle the exception
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_cancelSource != null)
                {
                    _listener.Stop();
                    _listener = null;
                    _cancelSource.Dispose();
                    _cancelSource = null;
                }
            }
        }
    }
}
