/******************************************************************************
 * Filename    = MokCommunicator.cs
 *
 * Author      = Manikanta
 *
 * Product     = Messenger
 * 
 * Project     = MessengerTests
 *
 * Description = Class that mocks the network communicator
 *****************************************************************************/

using MessengerNetworking.Communicator;
using MessengerNetworking.NotificationHandler;
using System.Net.Sockets;

namespace MessengerTests.ContentTests
{
    public class MockCommunicator : ICommunicator
    {
        public int ListenPort => throw new NotImplementedException();

        public string IpAddress => throw new NotImplementedException();
        private string _sendSerializedStr;

        public void AddClient(string ipAddress, int port)
        {
            throw new NotImplementedException();
        }

        public void AddSubscriber(string id, INotificationHandler subscriber)
        {
            throw new NotImplementedException();
        }

        public void Broadcast(string senderId, string message, int priority = 0)
        {
            _sendSerializedStr=message;
        }
        public string GetSendData()
        {
            return _sendSerializedStr;
        }
        public void RemoveClient(string ipAddress, int port)
        {
            throw new NotImplementedException();
        }

        public void RemoveSubscriber(string id)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string ipAddress, int port, string senderId, string message, int priority = 0)
        {
            throw new NotImplementedException();
        }
    }
}
