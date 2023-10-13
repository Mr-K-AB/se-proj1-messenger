using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessengerNetworking.Communicator
{
    public interface ICommunicator
    {
        public void Send(string serializedData, string senderModule, string destination);

        public void Broadcast(string serializedData, string senderModule);

        public void AddClient(string client, TcpClient clientSocket);

        public void RemoveClient(string client);
    }
}
