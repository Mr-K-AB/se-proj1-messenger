using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessengerNetworking.Communicator
{
    public class ServerCommunicator : ICommunicator
    {
        private Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
        private TcpListener listener;

        public ServerCommunicator(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine("Server started. Listening for incoming connections...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Thread clientThread = new Thread(start: HandleClient);
                clientThread.Start(client);
            }
        }

        public void Send(string serializedData, string senderModule, string destination)
        {
            if (clients.ContainsKey(destination))
            {
                TcpClient client = clients[destination];
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(serializedData);
                stream.Write(data, 0, data.Length);
            }
            else
            {
                Console.WriteLine($"Client '{destination}' not found.");
            }
        }

        public void Broadcast(string serializedData, string senderModule)
        {
            foreach (var client in clients)
            {
                Send(serializedData, senderModule, client.Key);
            }
        }

        public void AddClient(string client, TcpClient clientSocket)
        {
            clients[client] = clientSocket;
        }

        public void RemoveClient(string client)
        {
            if (clients.ContainsKey(client))
            {
                clients[client].Close();
                clients.Remove(client);
            }
        }

        private void HandleClient(object clientObj)
        {
            TcpClient client = (TcpClient)clientObj;
            NetworkStream stream = client.GetStream();
            string clientEndPoint = client.Client.RemoteEndPoint.ToString();

            Console.WriteLine($"Client connected: {clientEndPoint}");
            AddClient(clientEndPoint, client);

            byte[] buffer = new byte[1024];
            int bytesRead;

            while (true)
            {
                try
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Received data from {clientEndPoint}: {data}");
                    }
                }
                catch
                {
                    break;
                }
            }

            Console.WriteLine($"Client disconnected: {clientEndPoint}");
            RemoveClient(clientEndPoint);
        }
    }
}
