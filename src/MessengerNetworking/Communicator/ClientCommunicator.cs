using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessengerNetworking.Communicator
{
    public class ClientCommunicator
    {
        private TcpClient client;
        private string serverIp;
        private int serverPort;

        public ClientCommunicator(string serverIp, int serverPort)
        {
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            client = new TcpClient();
        }

        public void Connect()
        {
            client.Connect(serverIp, serverPort);
        }

        public void Send(string serializedData, string senderModule, string destination)
        {
            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(serializedData);
            stream.Write(data, 0, data.Length);
        }

        public void Broadcast(string serializedData, string senderModule)
        {
            Send(serializedData, senderModule, null);
        }

        public void AddClient(string client, TcpClient clientSocket)
        {

        }

        public void RemoveClient(string client)
        {

        }
    }
}
