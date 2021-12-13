using NetCoreServer;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TcpClient = NetCoreServer.TcpClient;

namespace LaaSender.Common.Network
{
    class ChatClient : TcpClient
    {
        public ChatClient(string address, int port) : base(address, port) 
        {
            
        }

        public void DisconnectAndStop()
        {
            _stop = true;
            DisconnectAsync();
            while (IsConnected)
                Thread.Yield();
        }

        protected override void OnConnected()
        {
            //Console.WriteLine($"Chat TCP client connected a new session with Id {Id}");
        }

        protected override void OnDisconnected()
        {
            //Console.WriteLine($"Chat TCP client disconnected a session with Id {Id}");

            // Wait for a while...
            Thread.Sleep(1000);

            if (_retryCount > 3)
            {
                _stop = true;
                DisconnectAsync();
                Thread.Yield();
            }

            _retryCount++;

            // Try to connect again
            if (!_stop)
                ConnectAsync();
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            //Console.WriteLine(Encoding.UTF8.GetString(buffer, (int)offset, (int)size));
        }

        protected override void OnError(SocketError error)
        {
            //Console.WriteLine($"Chat TCP client caught an error with code {error}");
        }

        private bool _stop;
        private int _retryCount = 0;
    }
}
