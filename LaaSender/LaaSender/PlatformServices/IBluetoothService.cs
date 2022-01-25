using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LaaSender
{
    public interface IBluetoothService
    {
        //void Connect(string name);
        void Connect(BluetoothDeviceInfo device);

        bool IsConnected();

        void Disconnect();

        //List<string> PairedDevices();
        List<BluetoothDeviceInfo> PairedDevices();

        void Send(string message);
    }
}
