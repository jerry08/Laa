﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using InTheHand.Net.Sockets;
using System.Linq;
using InTheHand.Net.Bluetooth;
using Laa.Shared;

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
            _serviceClassId = new Guid(LaaConstants.UUID);
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
                        //{
                        //    string content = streamReader.ReadLine();
                        //    //break;
                        //}

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
