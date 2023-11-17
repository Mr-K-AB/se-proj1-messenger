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
        private readonly bool _debug = false;


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
            _msgTrack = new Dictionary<Tuple<string, int>, Tuple<List<Tuple<string, string>>, int>>();
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
            Trace.TraceInformation("Networking constructor initialised");
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
            _msgTrack = new Dictionary<Tuple<string, int>, Tuple<List<Tuple<string, string>>, int>>();
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
            Trace.TraceInformation("Networking constructor initialised");
        }

        /// <inheritdoc />
        public int ListenPort { get; private set; }

        public string IpAddress { get; private set; }

        private bool IsIP10Range(string ipAddress)
        {
            IPAddress ip;
            if (IPAddress.TryParse(ipAddress, out ip))
            {
                byte[] bytes = ip.GetAddressBytes();
                if (bytes[0] == 10)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsIP172Dot16_12Range(string ipAddress)
        {
            IPAddress ip;
            if (IPAddress.TryParse(ipAddress, out ip))
            {
                byte[] bytes = ip.GetAddressBytes();
                if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsIP192Dot168(string ipAddress)
        {
            IPAddress ip;
            if (IPAddress.TryParse(ipAddress, out ip))
            {
                byte[] bytes = ip.GetAddressBytes();

                // Check for private IP address ranges
                if (bytes[0] == 192 && bytes[1] == 168)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsIPLocalhost(string ipAddress)
        {
            IPAddress ip;
            if (IPAddress.TryParse(ipAddress, out ip))
            {
                // Check for localhost
                if (ip.Equals(IPAddress.Loopback))
                {
                    return true; // Localhost
                }
            }
            return false;
        }

        private string getIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            string ipString = "127.0.0.1";
            foreach (IPAddress address in hostEntry.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork && IsIPLocalhost(address.ToString()))
                {
                    ipString = address.ToString();
                }
            }
            foreach (IPAddress address in hostEntry.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork && IsIP192Dot168(address.ToString()))
                {
                    ipString = address.ToString();
                }
            }
            foreach (IPAddress address in hostEntry.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork && IsIP172Dot16_12Range(address.ToString()))
                {
                    ipString = address.ToString();
                }
            }
            foreach (IPAddress address in hostEntry.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork && IsIP10Range(address.ToString()))
                {
                    ipString = address.ToString();
                }
            }
            Trace.TraceWarning("Networking : IP returned as " + ipString);
            return ipString;
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
            Trace.TraceInformation("Networking Subscriber added " + id);
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
            Trace.TraceInformation("Networking client added with ip and port " + ipAddress + port.ToString());
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
            Trace.TraceInformation("Networking client removed with ip and port " + ipAddress + port.ToString());
        }

        private static int FindFreePort()
        {
            TcpListener tcpListener = new(IPAddress.Loopback, 0);
            tcpListener.Start();

            int port =
                ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            Trace.TraceInformation("Networking free port found " + port.ToString());
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
            Trace.TraceInformation("Networking Subscriber Removed" + id);
        }

        /// <inheritdoc/>
        private void SendMessageWithPriority(string ipAddress, int port, string senderId, string message)
        {
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress broadcastAddress = IPAddress.Parse(ipAddress);
            byte[] sendBuffer = Encoding.ASCII.GetBytes($"{IpAddress}:{ListenPort}:{senderId}:{message}");
            IPEndPoint endPoint = new(broadcastAddress, port);
            if (_debug)
            {
                string filePath = "debug.txt";
                if (!File.Exists(filePath))
                {
                    using FileStream fs = File.Create(filePath);
                }
                using StreamWriter writer = new(filePath, true); // The 'true' parameter appends to an existing file
                writer.Write($"{IpAddress}:{ListenPort}:{senderId}:{message}\r");
            }
            else
            {
                // set buffer size to 128MB
                socket.SendBufferSize = 128 * 1024 * 1024;
                Debug.WriteLine(socket.SendBufferSize);
                int bytesSent = socket.SendTo(sendBuffer, endPoint);
                Debug.Assert(bytesSent == sendBuffer.Length);
                Trace.TraceInformation("Networking bytes sent " +  bytesSent.ToString() + " to endpoint " + endPoint.ToString());
            }
            _timeTrack.Add(new Tuple<string, int>(ipAddress, port), DateTime.Now);

            if (_msgTrack.ContainsKey(new Tuple<string, int>(ipAddress, port)))
            {
                Tuple<List<Tuple<string, string>>, int> value = _msgTrack[new Tuple<string, int>(ipAddress, port)];
                value.Item1.Add(new Tuple<string, string>(senderId, message));
                _msgTrack[new Tuple<string, int>(ipAddress, port)] = new Tuple<List<Tuple<string, string>>, int>(value.Item1, value.Item2);
            }
            else
            {
                List<Tuple<string, string>> value = new()
                {
                    new Tuple<string, string> (senderId, message)
                };
                _msgTrack.Add(new Tuple<string, int>(ipAddress, port), new Tuple<List<Tuple<string, string>>, int>(value, 0));
            }
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
                Trace.TraceInformation("Networking high priority message received");
            }
            else
            {
                _lowPriorityQueue.Enqueue(_content);
                Trace.TraceInformation("Networking low priority message received");
            }
        }


        public void Broadcast(string senderId, string message, int priority = 0)
        {
            foreach (Tuple<string, int> client in _clients)
            {
                SendMessage(client.Item1, client.Item2, senderId, message, priority);
            }
            Trace.TraceInformation("Networking message broadcasted");
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
                            Trace.TraceInformation("Networking control given to sendmessagepriority");
                        }
                        count = 2;
                        while (count > 0 && _lowPriorityQueue.Count > 0)
                        {
                            _queueContents _front = _lowPriorityQueue.Dequeue();
                            SendMessageWithPriority(_front._ipAddress, _front._port, _front._senderId, _front._message);
                            count--;
                            Trace.TraceInformation("Networking control given to sendmessagepriority");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    Trace.TraceError("Networking" + e.Message);
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
                            Trace.TraceInformation("Networking timeout detected");
                            if (_msgTrack.ContainsKey(item.Key))
                            {
                                Tuple<List<Tuple<string, string>>, int> msgToSend = _msgTrack[(item.Key)];
                                Trace.TraceInformation("Networking retry left for a message " +  msgToSend.Item2.ToString());
                                if (msgToSend.Item2 == 3)
                                {
                                    Trace.TraceInformation("Networking removed client from non activity");
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
                    string payload;
                    if (_debug)
                    {
                        string filePath = "debug.txt";
                        if (!File.Exists(filePath))
                        {
                            Console.WriteLine("File does not exist.");
                            payload = string.Empty;
                        }

                        string removedLine = string.Empty;

                        // Use List<string> to store lines without '\r'
                        List<string> linesWithoutFirstLine = new();

                        // Use StreamReader to read from the file
                        using (StreamReader reader = new(filePath))
                        {
                            string line;

                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line.Contains('\r'))
                                {
                                    removedLine = line;
                                }
                                else
                                {
                                    linesWithoutFirstLine.Add(line);
                                }
                                if (removedLine != string.Empty)
                                {
                                    break;
                                }
                            }
                        }
                        File.WriteAllLines(filePath, linesWithoutFirstLine);
                        payload = removedLine;
                    }
                    else
                    {
                        byte[] bytes = _listener.Receive(ref _endPoint);
                        payload = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                        Trace.TraceInformation("Networking received message");
                    }
                    Debug.WriteLine($"Received payload: {payload}");

                    // The received payload is expected to be in the format <Identity>:<Message>
                    string[] tokens = payload.Split(':', 4, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 4)
                    {
                        string ipAddress = tokens[0];
                        string port = tokens[1];
                        string id = tokens[2];
                        string message = tokens[3];

                        if (!_clients.Contains(new Tuple<string, int>(ipAddress, int.Parse(port))))
                        {
                            AddClient(ipAddress, int.Parse(port));
                        }
                        // lock (this)
                        {
                            Console.WriteLine("[" + id + "] : " + message);
                            if (_subscribers.ContainsKey(id))
                            {
                                _subscribers[id].OnDataReceived(message);
                            }
                            else
                            {
                                Debug.WriteLine($"Received message for unknown subscriber: {id}");
                                Trace.TraceWarning($"Netwroking Received message for unknown subscriber: {id}");
                            }
                            if (message != "ALIVE")
                            {
                                Trace.TraceInformation("Networking sent message alive");
                                SendMessage(ipAddress, int.Parse(port), "Networking", "ALIVE");
                            }
                            if (_timeTrack.ContainsKey(new Tuple<string, int>(ipAddress, int.Parse(port))))
                            {
                                _timeTrack.Remove(new Tuple<string, int>(ipAddress, int.Parse(port)));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    Trace.TraceError("Networking " + e.Message);
                }
            }
        }
    }
}
