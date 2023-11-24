/******************************************************************************
* Filename    = MockServerCommunicator.cs
*
* Author      = Pratham Ravindra Nagpure 
*
* Roll number = 112001054
*
* Product     = Messenger 
* 
* Project     = MessengerTests
*
* Description = A class for mocking the server communicator.
*****************************************************************************/

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
    internal class MockServerCommunicator : MockCommunicator
    {
        private readonly Dictionary<string, MockClientCommunicator> _idToClientCommunicatorMap = new();

        public override void Send(string serializedData, string moduleOfPacket, string? destination)
        {
            if (destination == null)
            {
                foreach(KeyValuePair<string, MockClientCommunicator> keyValuePair in _idToClientCommunicatorMap)
                {
                    keyValuePair.Value.OnDataReceived(serializedData, moduleOfPacket, destination);
                }
            }
            else
            {
                _idToClientCommunicatorMap[destination].OnDataReceived(serializedData, moduleOfPacket, destination);
            }
        }

        public void OnClientJoined()
        {
            foreach(KeyValuePair<string, INotificationHandler> keyValuePair in _moduleNameToNotificationHandlerMap)
            {
                INotificationHandler handler = keyValuePair.Value;
                handler.OnClientJoined(new TcpClient());
            }
        }

        public void OnClientLeft(string id)
        {
            foreach(KeyValuePair<string, INotificationHandler> keyValuePair in _moduleNameToNotificationHandlerMap)
            {
                INotificationHandler handler = keyValuePair.Value;
                handler.OnClientLeft(id);
            }
        }

        public void AddClientCommunicator(string id, MockClientCommunicator clientCommunicator)
        {
            _idToClientCommunicatorMap.Add(id, clientCommunicator);
        }
    }
}
