using LaaServer.ViewModels;
using NetCoreServer;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LaaServer.Network
{
    class EchoServer : UdpServer
    {
        public EchoServer(IPAddress address, int port) : base(address, port) { }

        protected override void OnStarted()
        {
            // Start receive datagrams
            ReceiveAsync();
        }

        protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            WifiViewModel.MessageReceived(message);
            SendAsync(endpoint, buffer, 0, size);
            return;

            Console.WriteLine("Incoming: " + Encoding.UTF8.GetString(buffer, (int)offset, (int)size));

            // Echo the message back to the sender
            SendAsync(endpoint, buffer, 0, size);
        }

        protected override void OnSent(EndPoint endpoint, long sent)
        {
            // Continue receive datagrams
            ReceiveAsync();
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Echo UDP server caught an error with code {error}");
        }
    }
}