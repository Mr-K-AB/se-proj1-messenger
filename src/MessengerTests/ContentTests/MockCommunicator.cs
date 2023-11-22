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

        public string IpAddress => throw new NotImplementedException();
        private string _sendSerializedStr;


        public void Send(string message, string senderId, string recieverid)
        {
            _sendSerializedStr=message;
        }
        public string GetSendData()
        {
            return _sendSerializedStr;
        }

        public string Start(string serverIP = null, string serverPort = null)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void AddClient(string clientId, TcpClient socket)
        {
            throw new NotImplementedException();
        }

        public void RemoveClient(string clientId)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(string moduleName, INotificationHandler notificationHandler, bool isHighPriority = false)
        {
            throw new NotImplementedException();
        }
    }
}
