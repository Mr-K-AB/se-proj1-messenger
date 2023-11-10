using System.ComponentModel;
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
        private readonly HashSet<Tuple<string, int>> _clients;
        private readonly UdpClient _listener;
        private readonly Thread _senderThread;      // Thread that sends message using priority queue.
        private readonly Thread _listenThread;      // Thread that listens for messages on the UDP port.
        private readonly Thread _timeoutThread;
        public readonly Dictionary<string, INotificationHandler> _subscribers; // List of subscribers.
        private readonly Queue<_queueContents> _highPriorityQueue, _lowPriorityQueue;
        private readonly Dictionary<Tuple<string, int>, DateTime> _timeTrack;
        private readonly Dictionary<Tuple<string, int>, Tuple<List<Tuple<string, string>>, int>> _msgTrack;
        private readonly int _timeOut = 60;



        /// <summary>
        /// Creates an instance of the UDP Communicator.
        /// </summary>
        /// <param name="listenPort">UDP port to listen on.</param>
        /// 
        public UdpCommunicator(int listenPort)
        {
            _subscribers = new Dictionary<string, INotificationHandler>();
            _clients = new HashSet<Tuple<string, int>>();

            // Create and start the thread that listens for messages.
            ListenPort = listenPort;
            IpAddress = getIPAddress();
            _highPriorityQueue = new Queue<_queueContents>();
            _lowPriorityQueue = new Queue<_queueContents>();
            _timeTrack = new Dictionary<Tuple<string, int>, DateTime>();
            _listener = new(ListenPort);
            _listenThread = new(new ThreadStart(ListenerThreadProc))
            {
                IsBackground = true // Stop the thread when the main thread stops.
            };
            _senderThread = new(new ThreadStart(SenderThreadProc))
            {
                IsBackground = true // Stop the thread when the main thread stops.
            };
            _timeoutThread = new(new ThreadStart(AliveChecker))
            {
                IsBackground = true
            };
            _listenThread.Start();
            _senderThread.Start();
        }
        public UdpCommunicator()
        {
            int listenPort = FindFreePort();
            _subscribers = new Dictionary<string, INotificationHandler>();
            _clients = new HashSet<Tuple<string, int>>();

            // Create and start the thread that listens for messages.
            ListenPort = listenPort;
            IpAddress = getIPAddress();
            _highPriorityQueue = new Queue<_queueContents>();
            _lowPriorityQueue = new Queue<_queueContents>();
            _timeTrack = new Dictionary<Tuple<string, int>, DateTime>();
            _listener = new(ListenPort);
            _listenThread = new(new ThreadStart(ListenerThreadProc))
            {
                IsBackground = true // Stop the thread when the main thread stops.
            };
            _senderThread = new(new ThreadStart(SenderThreadProc))
            {
                IsBackground = true // Stop the thread when the main thread stops.
            };
            _timeoutThread = new(new ThreadStart(AliveChecker))
            {
                IsBackground = true
            };
            _listenThread.Start();
            _senderThread.Start();
        }

        /// <inheritdoc />
        public int ListenPort { get; private set; }

        public string IpAddress { get; private set; }


        private string getIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            foreach (IPAddress address in hostEntry.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return address.ToString();
                }
            }
            return "127.0.0.1";
        }

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

        public void AddClient(string ipAddress, int port)
        {
            Debug.Assert(!string.IsNullOrEmpty(ipAddress));
            Debug.Assert(port != 0);

            // Assert valid Ip address
            _clients.Add(new Tuple<string, int>(ipAddress, port));

            foreach (KeyValuePair<string, INotificationHandler> subscriber in _subscribers)
            {
                subscriber.Value.OnClientJoined(ipAddress, port);
            }
        }

        public void RemoveClient(string ipAddress, int port)
        {
            Debug.Assert(!string.IsNullOrEmpty(ipAddress));
            Debug.Assert(port != 0);

            _clients.Remove(new Tuple<string, int>(ipAddress, port));

            foreach (KeyValuePair<string, INotificationHandler> subscriber in _subscribers)
            {
                subscriber.Value.OnClientLeft(ipAddress, port);
            }
        }

        private static int FindFreePort()
        {
            TcpListener tcpListener = new(IPAddress.Loopback, 0);
            tcpListener.Start();

            int port =
                ((IPEndPoint)tcpListener.LocalEndpoint).Port;
                tcpListener.Stop();
            return port;
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
            byte[] sendBuffer = Encoding.ASCII.GetBytes($"{ipAddress}:{port}:{senderId}:{message}");
            IPEndPoint endPoint = new(broadcastAddress, port);
            int bytesSent = socket.SendTo(sendBuffer, endPoint);
            _timeTrack.Add(new Tuple<string, int> (ipAddress, port), DateTime.Now);

            if (_msgTrack.ContainsKey(new Tuple<string, int> (ipAddress, port)))
            {
                Tuple<List<Tuple<string, string>>, int> value = _msgTrack[new Tuple<string, int>(ipAddress, port)];
                value.Item1.Add(new Tuple<string, string> (senderId, message));
                _msgTrack[new Tuple<string, int>(ipAddress, port)] = new Tuple<List<Tuple<string, string>>, int> (value.Item1, value.Item2);
            }
            else
            {
                List<Tuple<string, string>> value = new()
                {
                    new Tuple<string, string> (senderId, message)
                };
                _msgTrack.Add(new Tuple<string, int>(ipAddress, port), new Tuple<List<Tuple<string, string>>, int>(value, 0));
            }
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
            Debug.Print("UdpCommunicator.SendMessage: {0}", message);
            if (priority == 1)
            {
                _highPriorityQueue.Enqueue(_content);
            }
            else
            {
                _lowPriorityQueue.Enqueue(_content);
            }
        }


        public void Broadcast(string senderId, string message, int priority = 0)
        {
            foreach (Tuple<string, int> client in _clients)
            {
                SendMessage(client.Item1, client.Item2, senderId, message, priority);
            }
        }

        private void SenderThreadProc()
        {
            Debug.WriteLine($"Sender Thread Id = {Environment.CurrentManagedThreadId}.");

            while (true)
            {
                try
                {
                    // lock (this)
                    {
                        // Object reference not set to an instance of an object.
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


        private void AliveChecker()
        {
            while (true)
            {
                lock (this)
                {
                    foreach (KeyValuePair<Tuple<string, int>, DateTime> item in _timeTrack)
                    {
                        if (item.Value - DateTime.Now > new TimeSpan(0, 1, 0))
                        {
                            if(_msgTrack.ContainsKey(item.Key))
                            {
                                Tuple<List<Tuple<string, string>>, int> msgToSend = _msgTrack[(item.Key)];
                                if (msgToSend.Item2 == 3)
                                {
                                    RemoveClient(item.Key.Item1, item.Key.Item2);
                                    _timeTrack.Remove(item.Key);
                                }
                                else
                                {
                                    foreach (Tuple<string, string> toSend in msgToSend.Item1)
                                    {
                                        SendMessage(item.Key.Item1, item.Key.Item2, toSend.Item1, toSend.Item2);
                                    }
                                    int count = _msgTrack[(item.Key)].Item2 + 1;
                                    _msgTrack[(item.Key)] = new Tuple<List<Tuple<string, string>>, int>(msgToSend.Item1, count);
                                }
                            }
                        }
                    }
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
                    string[] tokens = payload.Split(':', 4, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 4)
                    {
                        string ipAddress = tokens[0];
                        string port = tokens[1];
                        string id = tokens[2];
                        string message = tokens[3];

                        if (! _clients.Contains(new Tuple<string, int> (ipAddress, int.Parse(port))))
                        {
                            AddClient(ipAddress, int.Parse(port));
                        }
                        // lock (this)
                        {
                            Console.WriteLine("[" +  id + "] : " + message);
                            if (_subscribers.ContainsKey(id))
                            {
                                _subscribers[id].OnDataReceived(message);
                            }
                            else
                            {
                                Debug.WriteLine($"Received message for unknown subscriber: {id}");
                            }
                            if (message != "ALIVE")
                            {
                                SendMessage(ipAddress, int.Parse(port), "Networking", "ALIVE");
                            }
                            if (_timeTrack.ContainsKey(new Tuple<string, int> (ipAddress, int.Parse(port))))
                            {
                                _timeTrack.Remove(new Tuple<string, int>(ipAddress, int.Parse(port)));
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
