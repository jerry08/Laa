using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LaaSender
{
    public interface IBluetoothService
    {
        void Connect(string name);

        bool IsEnabled();
        
        Task<bool> Enable();

        bool IsConnected();

        void Disconnect();

        List<string> PairedDevices();

        void Send(string message);
    }
}
