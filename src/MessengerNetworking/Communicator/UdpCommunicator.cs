/******************************************************************************
 * Filename    = UdpCommunicator.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = Networking
 *
 * Description = Defines a UDP communicator.
 *****************************************************************************/

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MessengerNetworking.NotificationHandler;

namespace MessengerNetworking.Communicator
{
    /// <summary>
    /// Communicator that can send and listen for messages over the network using UDP.
    /// </summary>
    public class UdpCommunicator : ICommunicator
    {
        private struct _queueContents
        {
            public string _ipAddress;
            public int _port;
            public string _senderId;
            public string _message;
            public int _priority;
        }
        private IPEndPoint? _endPoint;
        private readonly UdpClient _listener;
        private readonly Thread _senderThread;      // Thread that sends message using priority queue.
        private readonly Thread _listenThread;      // Thread that listens for messages on the UDP port.
        public readonly Dictionary<string, INotificationHandler> _subscribers; // List of subscribers.
        private readonly Queue<_queueContents> _highPriorityQueue, _lowPriorityQueue;
        

        /// <summary>
        /// Creates an instance of the UDP Communicator.
        /// </summary>
        /// <param name="listenPort">UDP port to listen on.</param>
        public UdpCommunicator(int listenPort)
        {
            _subscribers = new Dictionary<string, INotificationHandler>();

            // Create and start the thread that listens for messages.
            ListenPort = listenPort;
            _listener = new(ListenPort);
            _listenThread = new(new ThreadStart(ListenerThreadProc))
            {
                IsBackground = true // Stop the thread when the main thread stops.
            };
            _senderThread = new(new ThreadStart(SenderThreadProc))
            {
                IsBackground = true // Stop the thread when the main thread stops.
            };
            _listenThread.Start();
            _senderThread.Start();
        }

        /// <inheritdoc />
        public int ListenPort { get; private set; }

        /// <inheritdoc />
        public void AddSubscriber(string id, INotificationHandler subscriber)
        {
            Debug.Assert(!string.IsNullOrEmpty(id));
            Debug.Assert(subscriber != null);

            lock (this)
            {
                if (_subscribers.ContainsKey(id))
                {
                    _subscribers[id] = subscriber;
                }
                else
                {
                    _subscribers.Add(id, subscriber);
                }
            }
        }

        /// <inheritdoc />
        public void RemoveSubscriber(string id)
        {
            Debug.Assert(!string.IsNullOrEmpty(id));

            lock (this)
            {
                if (_subscribers.ContainsKey(id))
                {
                    _subscribers.Remove(id);
                }
            }
        }

        /// <inheritdoc/>
        private void SendMessageWithPriority(string ipAddress, int port, string senderId, string message)
        {
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress broadcastAddress = IPAddress.Parse(ipAddress);
            byte[] sendBuffer = Encoding.ASCII.GetBytes($"{senderId}:{message}");
            IPEndPoint endPoint = new(broadcastAddress, port);
            int bytesSent = socket.SendTo(sendBuffer, endPoint);
            Debug.Assert(bytesSent == sendBuffer.Length);
        }

        public void SendMessage(string ipAddress, int port, string senderId, string message, int priority = 0)
        {
            _queueContents _content;
            _content._ipAddress = ipAddress;
            _content._port = port;
            _content._senderId = senderId;
            _content._message = message;
            _content._priority = priority;
            if (priority == 1)
            {
                _highPriorityQueue.Enqueue(_content);
            }
            else
            {
                _lowPriorityQueue.Enqueue(_content);
            }
        }

        private void SenderThreadProc()
        {
            Debug.WriteLine($"Sender Thread Id = {Environment.CurrentManagedThreadId}.");

            while (true)
            {
                try
                {
                    lock (this)
                    {
                        int count = 5;
                        while (count > 0 && _highPriorityQueue.Count > 0)
                        {
                            _queueContents _front = _highPriorityQueue.Dequeue();
                            SendMessageWithPriority(_front._ipAddress, _front._port, _front._senderId, _front._message);
                            count--;
                        }
                        count = 2;
                        while (count > 0 && _lowPriorityQueue.Count > 0)
                        {
                            _queueContents _front = _lowPriorityQueue.Dequeue();
                            SendMessageWithPriority(_front._ipAddress, _front._port, _front._senderId, _front._message);
                            count--;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }


        /// <summary>
        /// Listens for messages on the listening port.
        /// </summary>
        private void ListenerThreadProc()
        {
            Debug.WriteLine($"Listener Thread Id = {Environment.CurrentManagedThreadId}.");

            while (true)
            {
                try
                {
                    // Listen for message on the listening port, and receive it when it comes along.
                    byte[] bytes = _listener.Receive(ref _endPoint);
                    string payload = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    Debug.WriteLine($"Received payload: {payload}");

                    // The received payload is expected to be in the format <Identity>:<Message>
                    string[] tokens = payload.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 2)
                    {
                        string id = tokens[0];
                        string message = tokens[1];
                        lock (this)
                        {
                            if (_subscribers.ContainsKey(id))
                            {
                            //    _subscribers[id].OnMessageReceived(message);
                            }
                            else
                            {
                                Debug.WriteLine($"Received message for unknown subscriber: {id}");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }
    }
}
