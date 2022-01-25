using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
//using InTheHand.Bluetooth;
using System.Net.Sockets;
using System.Diagnostics;

namespace LaaSender
{
	class BluetoothClientTest
	{
		private Guid UUID;
		private BluetoothClient bluetoothClient;

		private string stringSend;

		public BluetoothClientTest()
		{
			UUID = new Guid(Laa.Shared.LaaConstants.UUID);

			bluetoothClient = new BluetoothClient();
			//List<string> items = new List<string>();
			//var devices = bluetoothClient.DiscoverDevices().ToList();
			var pairedDevices = bluetoothClient.PairedDevices.ToList();
			//BluetoothDeviceInfo[] devices = bluetoothClient.DiscoverDevicesInRange();
			//foreach (BluetoothDeviceInfo d in devices)
			//{
			//    items.Add(d.DeviceName);
			//}

			try
			{
				if (pairedDevices[1].Authenticated)
				{
					bluetoothClient.Connect(pairedDevices[1].DeviceAddress, UUID);
				}

				List<string> data = new List<string>()
				{
					"ts1 " + Laa.Shared.LaaConstants.FirstHash
				};
				string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

				//Stream stream = bluetoothClient.GetStream();
				
				//StreamWriter sw = new StreamWriter(stream, Encoding.UTF8);
				//sw.WriteLine(json);
				//sw.Close();

				for (int i = 0; i < 30; i++)
                {
					Stopwatch ss = new Stopwatch();
					ss.Start();
					bluetoothClient.Connect(pairedDevices[1].DeviceAddress, UUID);
					ss.Stop();

					if (ss.ElapsedMilliseconds > 400)
                    {

                    }

					using (NetworkStream stream = bluetoothClient.GetStream())
					using (StreamWriter sw = new StreamWriter(stream, Encoding.UTF8))
					{
						sw.WriteLine(json);
					}
					
					//byte[] sent2 = Encoding.UTF8.GetBytes(json);
					//stream.Write(sent2, 0, sent2.Length);
					//stream.Flush();

					//pairedDevices[1].Refresh();

					Thread.Sleep(50);
				}

				return;

				//byte[] sent = Encoding.UTF8.GetBytes("testing 12" + Laa.Shared.LaaConstants.FirstHash);
				//stream.Write(sent, 0, sent.Length);
				//stream.Flush();
			}
			catch (Exception e)
			{

			}
		}

		public void start(BluetoothDeviceInfo serverToSend)
		{
			try
			{
				if (serverToSend.Authenticated)
				{
					bluetoothClient = new BluetoothClient();
					bluetoothClient.Connect(serverToSend.DeviceAddress, UUID);
					Console.WriteLine("Conexão Estabelecida!");

					Thread thread = new Thread(new ThreadStart(threadClientBluetooth));
					thread.Start();
				}
				else
				{
					Console.WriteLine("Servidor Não Autenticado!");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Error: " + e.ToString());
			}
		}

		public void threadClientBluetooth()
		{
			try
			{
				Stream stream = bluetoothClient.GetStream();

				byte[] sent = Encoding.ASCII.GetBytes(stringSend);
				stream.Write(sent, 0, sent.Length);
				stream.Flush();

				Console.WriteLine("Mensagem Enviada!");
			}
			catch (Exception e)
			{
				Console.WriteLine("Erro: " + e.ToString());
			}
		}

		public void close()
		{
			bluetoothClient.Close();
			bluetoothClient.Dispose();
			Console.WriteLine("Conexão terminada!");
		}

		public void setStringSend(String status)
		{
			this.stringSend = stringSend;
		}

		public String getStringSend()
		{
			return stringSend;
		}
	}
}
