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
    internal class MockCommunicator : ICommunicator
    {

        protected readonly Dictionary<string, INotificationHandler> _moduleNameToNotificationHandlerMap = new();

        public virtual void AddClient(string clientId, TcpClient socket)
        {
        }

        public virtual void RemoveClient(string clientId)
        {
        }

        public virtual void Send(string serializedData, string moduleOfPacket, string? destination)
        {
        }

        public virtual string Start(string serverIP = null, string serverPort = null)
        {
            return "0.0.0.0:0";
        }

        public virtual void Stop()
        {
        }

        public virtual void Subscribe(string moduleName, INotificationHandler notificationHandler, bool isHighPriority = false)
        {
            _moduleNameToNotificationHandlerMap.Add(moduleName, notificationHandler);
        }

        public virtual void OnDataReceived(string serializedData, string moduleOfPacket, string? destination)
        {
            INotificationHandler handler = _moduleNameToNotificationHandlerMap[moduleOfPacket];
            handler.OnDataReceived(serializedData);
        }

    }
}
