using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerNetworking.Communicator
{
    public interface IMessageListener
    {
        /// <summary>
        /// Declares a listener for messages from the communicator.
        /// </summary>
        public interface IMessageListener
        {
            /// <summary>
            /// Handles reception of a message.
            /// </summary>
            /// <param name="message">Message that is received</param>
            void OnMessageReceived(string message);
        }
    }
}
