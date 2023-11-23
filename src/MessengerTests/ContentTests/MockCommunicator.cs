/******************************************************************************
 * Filename    = MockCommunicator.cs
 *
 * Author      = Rapeti Siddhu Neehal
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
using System.Windows.Markup;

namespace MessengerTests.ContentTests
{
    public class MockCommunicator : ICommunicator
    {
        private bool _isBroadcast;
        private string _sendSerializedStr;
        private readonly List<INotificationHandler> _subscribers;

        public MockCommunicator()
        {
            _sendSerializedStr = "";
            _subscribers = new List<INotificationHandler>();
        }
        public void Send(string message, string senderId, string recieverid)
        {
            if (recieverid == null)
            {
                _sendSerializedStr = message;
                _isBroadcast = true;
            }
            else
            {
                _sendSerializedStr = message;
                _isBroadcast = false;
            }
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
            _subscribers.Add(notificationHandler);
        }

        public void Reset()
        {
            _isBroadcast = false;
        }
        public bool IsBroadcast()
        {
            bool flag = _isBroadcast;
            Reset();
            return flag;
        }
    }
}
