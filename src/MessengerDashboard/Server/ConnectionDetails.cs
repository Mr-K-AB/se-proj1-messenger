using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace MessengerDashboard.Server
{
    public class ConnectionDetails
    {
        public int Port { get; init; }

        public string IpAddress { get; init; }

        public ConnectionDetails(string ipAddress, int port)
        {
            Port = port;
            IpAddress = ipAddress;
        }
    }
}
