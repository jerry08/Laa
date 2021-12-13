using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaaServer.Common.Services
{
    /// <summary>
    /// Define the receiver Bluetooth service interface.
    /// </summary>
    public interface IReceiverBluetoothService
    {
        /// <summary>
        /// Gets a value indicating whether was started.
        /// </summary>
        /// <value>
        /// The was started.
        /// </value>
        bool WasStarted { get; }

        /// <summary>
        /// Starts the listening from Senders.
        /// </summary>
        /// <param name="reportAction">
        /// The report Action.
        /// </param>
        void Start(Action<string> reportAction);

        /// <summary>
        /// Stops the listening from Senders.
        /// </summary>
        void Stop();
    }
}
