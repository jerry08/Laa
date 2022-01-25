using InTheHand.Net.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaaSender
{
    public class BluetoothService : IBluetoothService
    {
        BluetoothClient BluetoothClient;
		Guid UUID = Guid.Parse(Laa.Shared.LaaConstants.UUID);

		CancellationTokenSource _cancellationToken;

		List<string> Messages = new List<string>();

		public BluetoothService()
        {
			//Automatically turns on bluetooth (Android, IOS?)
            BluetoothClient = new BluetoothClient();
        }

        public void Connect(BluetoothDeviceInfo device)
        {
			Task.Run(async () => await ConnectDevice(device));
        }

		private async Task ConnectDevice(BluetoothDeviceInfo device)
		{
			_cancellationToken = new CancellationTokenSource();

			while (_cancellationToken.IsCancellationRequested == false)
			{
				try
                {
					await Task.Delay(250);

					if (device.Authenticated)
					{
						BluetoothClient.Connect(device.DeviceAddress, UUID);
					}

					if (BluetoothClient.Connected)
					{
						while (_cancellationToken.IsCancellationRequested == false)
						{
							if (Messages.Count > 0)
							{
								string json = JsonConvert.SerializeObject(Messages);
								Messages.Clear();

								//var msgBytes = Encoding.UTF8.GetBytes(json);

								//Stream stream = bluetoothClient.GetStream();

								//StreamWriter sw = new StreamWriter(stream, Encoding.UTF8);
								//sw.WriteLine(json);
								//sw.Close();

								using (NetworkStream stream = BluetoothClient.GetStream())
								{
									if (stream != null)
									{
										using (StreamWriter sw = new StreamWriter(stream, Encoding.UTF8))
										{
											sw.WriteLine(json);
										}
									}
								}

								//byte[] sent2 = Encoding.UTF8.GetBytes(json);
								//stream.Write(sent2, 0, sent2.Length);
								//stream.Flush();

								//byte[] sent = Encoding.UTF8.GetBytes("testing 12" + Laa.Shared.LaaConstants.FirstHash);
								//stream.Write(sent, 0, sent.Length);
								//stream.Flush();

								if (device.Authenticated)
								{
									BluetoothClient.Connect(device.DeviceAddress, UUID);
								}
							}
						}
					}
				}
				catch (Exception e)
                {
					Debug.Write(e);
					Debug.Write(e.Message);
				}
				finally
                {

                }
			}
		}

		public void Disconnect()
        {
            BluetoothClient.Close();
        }

        public bool IsConnected()
        {
            return BluetoothClient.Connected;
        }

        public List<BluetoothDeviceInfo> PairedDevices()
        {
            return BluetoothClient.PairedDevices.ToList();
        }

        public void Send(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Messages.Add(message);
            }
        }
    }
}