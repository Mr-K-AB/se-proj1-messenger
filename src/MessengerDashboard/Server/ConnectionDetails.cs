/******************************************************************************
* Filename    = ConnectionDetails.cs
*
* Author      = Pratham Ravindra Nagpure 
*
* Roll number = 112001054
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = This file contains the definition of the ConnectionDetails class, 
*               which represents the details required for establishing a connection.
*****************************************************************************/

namespace MessengerDashboard.Server
{
    /// <summary>
    /// Represents the details required for establishing a connection.
    /// </summary>
    public class ConnectionDetails
    {
        /// <summary>
        /// Gets the port number for the connection.
        /// </summary>
        public int Port { get; init; }

        /// <summary>
        /// Gets the IP address for the connection.
        /// </summary>
        public string IpAddress { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionDetails"/> class with the specified IP address and port.
        /// </summary>
        /// <param name="ipAddress">The IP address.</param>
        /// <param name="port">The port number.</param>
        public ConnectionDetails(string ipAddress, int port)
        {
            Port = port;
            IpAddress = ipAddress;
        }
    }
}

