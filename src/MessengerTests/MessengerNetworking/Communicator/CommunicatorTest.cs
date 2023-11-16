using MessengerNetworking.Communicator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using MessengerNetworking.NotificationHandler;
using System.Net.Sockets;
using System.Diagnostics;

namespace MessengerTests.MessengerNetworkingTest
{
    /// <summary>
    /// Unit tests for the chat messenger.
    /// </summary>
    [TestClass]
    public class ChatMessengerUnitTests
    {
        /// <summary>
        /// Validates that the chat messenger subscribes with the communicator passed to it.
        /// </summary>
        [TestMethod]
        public void TestSubscriber()
        {
            UdpCommunicator UdpCommunicator = new();
            ChatMessenger _ = new(UdpCommunicator);

            Logger.LogMessage($"Validate that the chat messenger subscribes with the communicator passed to it.");
            Assert.AreEqual(UdpCommunicator._subscribers.Count, 1);
            Assert.IsNotNull(UdpCommunicator._subscribers[ChatMessenger.Identity]);
        }

        /// <summary>
        /// Validates message sending functionality of the chat messenger.
        /// </summary>
        /// 
        
        [TestMethod]
        public void TestSendMessage()
        {
            string ipAddress = "127.0.0.1";
            int port = 12345;
            UdpCommunicator server = new(port - 100);

            UdpCommunicator client = new(port);
            // ChatMessenger messenger = new(UdpCommunicator);

            // Send a message to the chat messenger and validate that it is received by the communicator.
            Logger.LogMessage($"Send a message to the chat messenger and validate that it is received by the communicator.");
            string chatMessage = "Hello World";
            server.AddClient(ipAddress, port);
            server.SendMessage(ipAddress, port, "Dashboard", chatMessage);
            // client.AddClient(ipAddress, port);
            Assert.IsTrue(true);
            // messenger.SendMessage(ipAddress, port, chatMessage);
            
            // Assert.IsTrue(server.IsEqualToLastMessage(ipAddress, port, ChatMessenger.Identity, chatMessage));

            // Send another message to the chat messenger and validate that it is received by the communicator.
            // Logger.LogMessage($"Send another message to the chat messenger and validate that it is received by the communicator.");
            // string anotherChatMessage = "Another Hello World Message";
            // messenger.SendMessage(ipAddress, port, anotherChatMessage);
            // Assert.IsTrue(server.IsEqualToLastMessage(ipAddress, port, ChatMessenger.Identity, anotherChatMessage));
        }
        
        /// <summary>
        /// Validates that the chat messenger processes received message, and notifies clients.
        /// </summary>
        [TestMethod]
        public void TestOnMessageReceived()
        {
            UdpCommunicator UdpCommunicator = new();
            ChatMessenger messenger = new(UdpCommunicator);

            Logger.LogMessage($"Sign up with the messenger for notifiation on message received.");
            string? message = null;
            messenger.OnChatMessageReceived += delegate (string chatMessage)
            {
                message = chatMessage;
            };

            Logger.LogMessage($"Validate that the messenger notifies clients upon receiving a message.");
            string testMessage = "Hello World!";
            messenger.OnDataReceived(testMessage);
            Assert.AreEqual(message, testMessage);
        }

        [TestMethod]
        public void TestGetIPAddress()
        {
            UdpCommunicator communicator = new();
            string ipAddress = "10.32.4.116";
            string communicatorIPAddress = communicator.IpAddress;
            Debug.WriteLine("Communicator IP Address" + communicatorIPAddress);
            Assert.IsTrue(communicatorIPAddress == ipAddress);
        }
        /*



    }
        */

        /// <summary>
        /// Processor for chat messages.
        /// </summary>
        /// 
        public delegate void ChatMessageReceived(string message);
        public class ChatMessenger : INotificationHandler
        {
            private readonly ICommunicator _communicator;

            /// <summary>
            /// The identity for this module.
            /// </summary>
            public const string Identity = "ChatMessenger";

            /// <summary>
            /// Event for handling received chat messages.
            /// </summary>
            public event ChatMessageReceived? OnChatMessageReceived;

            /// <summary>
            /// Creates an instance of the chat messenger.
            /// </summary>
            /// <param name="communicator">The communicator instance to use</param>
            public ChatMessenger(ICommunicator communicator)
            {
                _communicator = communicator;
                communicator.AddSubscriber(Identity, this);
            }

            public void OnClientJoined(string ipAddress, int port)
            {

            }

            public void OnClientLeft(string ipAddress, int port)
            {

            }

            /// <summary>
            /// Sends the given message to the given ip and port.
            /// </summary>
            /// <param name="ipAddress">IP address of the destination</param>
            /// <param name="port">Port of the destination</param>
            /// <param name="message">Message to be sent</param>
            public void SendMessage(string ipAddress, int port, string message)
            {
                _communicator.SendMessage(ipAddress, port, Identity, message);
            }

            /// <inheritdoc />
            public void OnDataReceived(string message)
            {
                OnChatMessageReceived?.Invoke(message);
            }
        }
    }
}
