using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MessengerNetworking.Communicator;
using MessengerNetworking.NotificationHandler;

namespace MessengerTests.DashboardTests.Mocks
{
    internal class MockClientCommunicator : MockCommunicator
    {
        private MockServerCommunicator _serverCommunicator;

        public override void Send(string serializedData, string moduleOfPacket, string? destination = null)
        {
            _serverCommunicator.OnDataReceived(serializedData, moduleOfPacket, destination);
        }

        public override string Start(string serverIP = null, string serverPort = null)
        {
            _serverCommunicator.OnClientJoined();
            return "0.0.0.0:0";
        }

        public void SetServer(MockServerCommunicator serverCommunicator)
        {
            _serverCommunicator = serverCommunicator;
        }
    }
}
