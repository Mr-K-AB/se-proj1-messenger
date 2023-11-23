/******************************************************************************
 * 
 * Author      = Priyanshu Gupta
 *
 * Roll no     = 112001033
 *
 *****************************************************************************/

using MessengerNetworking.Communicator;
using System;
using System.Diagnostics;

namespace MessengerNetworking.Factory
{
    public static class CommunicationFactory
    {
        private static readonly CommunicatorClient s_communicatorClient =
            new();
        private static readonly CommunicatorServer s_communicatorServer =
            new();

        /// <summary>
        /// Factory function to get the communicator.
        /// </summary>
        /// <param name="isClientSide">
        /// Boolean telling is it client side or server side.
        /// </param>
        /// <returns> The communicator singleton instance. </returns>
        public static ICommunicator GetCommunicator(
            bool isClientSide = true)
        {
            Trace.WriteLine("[Networking] " +
                "CommunicationFactory.GetCommunicator() function " +
                "called with isClientSide: " + isClientSide.ToString());
            if (isClientSide)
            {
                return s_communicatorClient;
            }
            return s_communicatorServer;
        }
    }
}
