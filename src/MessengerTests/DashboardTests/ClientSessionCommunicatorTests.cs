/******************************************************************************
* Filename    = ClientSessionCommunicatorTests.cs
*
* Author      = Shailab Chauhan 
*
* Roll number = 112001038
*
* Product     = Messenger 
* 
* Project     = MessengerTests
*
* Description = A class to test the client session controller.
*****************************************************************************/


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerContent.DataModels;
using MessengerDashboard;
using MessengerDashboard.Client;
using MessengerDashboard.Client.Events;
using MessengerDashboard.Server;
using MessengerTests.DashboardTests.Mocks;

namespace MessengerTests.DashboardTests
{
    [TestClass]
    public class ClientSessionCommunicatorTests
    {
        ServerSessionController _server;
        ClientSessionController _client1;
        ClientSessionController _client;

        [TestMethod]
        public void SessionEmptyNameTest()
        {
            MockServerCommunicator serverCommunicator = new();
            MockClientCommunicator clientCommunicator1 = new();
            MockClientCommunicator clientCommunicator2 = new();
            serverCommunicator.AddClientCommunicator("1", clientCommunicator1);
            serverCommunicator.AddClientCommunicator("2", clientCommunicator2);
            clientCommunicator1.SetServer(serverCommunicator);
            clientCommunicator2.SetServer(serverCommunicator);
            MockContentServer contentServer = new();

            _server = new(serverCommunicator, contentServer);


            _client1 = new(clientCommunicator1, new MockContentClient(), new MockScreenShareClient());

            ConnectionDetails connectionDetails = _server.ConnectionDetails;
            if (_client1.ConnectToServer(connectionDetails.IpAddress, connectionDetails.Port, "", "1@gmail.com", "photo1"))
            {
                Assert.Fail("Client 1 got connect which is wrong");
            }
        }

        [TestMethod]
        public void EdgeCasesTest()
        {
            MockServerCommunicator serverCommunicator = new();
            MockClientCommunicator clientCommunicator = new();
            serverCommunicator.AddClientCommunicator("1", clientCommunicator);
            clientCommunicator.SetServer(serverCommunicator);
            MockContentServer contentServer = new();
            _server = new (serverCommunicator, contentServer);
            _client = new(clientCommunicator, new MockContentClient(), new MockScreenShareClient());

            try
            {
                serverCommunicator.Send(null, "Dashboard", "1");
                serverCommunicator.Send("dghfgsgjh", "Dashboard", "1");
                ServerPayload payload = new()
                {
                    Operation = Operation.TakeUserDetails
                };

                Serializer serializer = new();
                string payload1 = serializer.Serialize(payload);
                serverCommunicator.Send(payload1, "Dashboard", "1");

                serverCommunicator.Send(null, "Dashboard", "1");
                serverCommunicator.Send("dghfgsgjh", "Dashboard", "1");
                ServerPayload payload2 = new()
                {
                    Operation = Operation.SessionUpdated
                };

                Serializer serializer1 = new();
                string payload3 = serializer.Serialize(payload2);
                serverCommunicator.Send(payload3, "Dashboard", "1");

                serverCommunicator.Send(null, "Dashboard", "1");
                serverCommunicator.Send("dghfgsgjh", "Dashboard", "1");
                ServerPayload payload4 = new()
                {
                    Operation = Operation.GiveUserDetails
                };

                Serializer serializer2 = new();
                string payload5 = serializer.Serialize(payload4);
                serverCommunicator.Send(payload5, "Dashboard", "1");

                

                serverCommunicator.Send(null, "Dashboard", "1");
                serverCommunicator.Send("dghfgsgjh", "Dashboard", "1");
                ServerPayload payload6 = new()
                {
                    Operation = Operation.Refresh
                };

                Serializer serializer3 = new();
                string payload7 = serializer.Serialize(payload6);
                serverCommunicator.Send(payload7, "Dashboard", "1");


                ClientSessionController controller = new();
                controller.ConnectToServer("12as", 0000000000000000000000000, "name", "sg@gmail.com", "");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void JoinAndLeaveTest()
        {
            MockServerCommunicator serverCommunicator = new ();
            MockClientCommunicator clientCommunicator1 = new ();
            serverCommunicator.AddClientCommunicator("1", clientCommunicator1);
            clientCommunicator1.SetServer(serverCommunicator);
            MockContentServer contentServer = new ();
            List<ChatThread> chatThreads = new()
            {
                new() { MessageList = new List<ReceiveChatData>() { new() { SenderID = 1, Data = "hello I am good",
                        Type = MessengerContent.MessageType.Chat } } },
                new() { MessageList = new List<ReceiveChatData>() { new() { SenderID = 1, Data = "I am in a bad mood",
                        Type = MessengerContent.MessageType.Chat } } },
                new() { MessageList = new List<ReceiveChatData>() { new() { SenderID = 1, Data = "What an achivement!",
                        Type = MessengerContent.MessageType.Chat } } },
            };

            _server = new (serverCommunicator, contentServer);

            _client1 = new (clientCommunicator1, new MockContentClient(),new MockScreenShareClient());

            ConnectionDetails connectionDetails = _server.ConnectionDetails;
            if (!_client1.ConnectToServer(connectionDetails.IpAddress, connectionDetails.Port, "Name1", "1@gmail.com", "photo1"))
            {
                Assert.Fail("Client 1 was not able to connect to the server");
            }
            Thread.Sleep(300);
            _client1.SendExamModeRequestToServer();
            Thread.Sleep(300);
            Assert.AreEqual(_client1.SessionInfo.SessionMode, SessionMode.Exam);
            _client1.SendLabModeRequestToServer();
            Thread.Sleep(300);
            Assert.AreEqual(_client1.SessionInfo.SessionMode, SessionMode.Lab);
            contentServer.SetChats(chatThreads);
            _client1.SendRefreshRequestToServer();
            _client1.SendExitSessionRequestToServer();
            Thread.Sleep(300);
            Assert.IsFalse(_client1.IsConnectedToServer);
        }

        [TestMethod]
        public void TestForNullEventArgs()
        {
            ClientSessionChangedEventArgs e1 = new(new());
            Assert.IsNotNull(e1);
            Assert.IsNotNull(e1.Session);
            SessionExitedEventArgs e2 = new(new(), new(), new());
            Assert.IsNotNull(e2);
            Assert.IsNotNull(e2.Sentiment);
            Assert.IsNotNull(e2.TelemetryAnalysis);
            Assert.IsNotNull(e2.Summary);
            SessionModeChangedEventArgs e3  = new (SessionMode.Lab);
            Assert.IsNotNull(e3);
            Assert.AreEqual(e3.SessionMode, SessionMode.Lab);
            RefreshedEventArgs e4 = new(new(), new(), new(), new());
            Assert.IsNotNull(e4);
            Assert.IsNotNull(e4.Sentiment);
            Assert.IsNotNull(e4.TelemetryAnalysis);
            Assert.IsNotNull(e4.Summary);
            Assert.IsNotNull(e4.SessionInfo);
        }

        [TestMethod]
        public void SessionJoinAndLeaveTest()
        {
            MockServerCommunicator serverCommunicator = new();
            MockClientCommunicator clientCommunicator1 = new();
            MockClientCommunicator clientCommunicator2 = new();
            serverCommunicator.AddClientCommunicator("1", clientCommunicator1);
            serverCommunicator.AddClientCommunicator("2", clientCommunicator2);
            clientCommunicator1.SetServer(serverCommunicator);
            clientCommunicator2.SetServer(serverCommunicator);
            MockContentServer contentServer = new();

            _server = new(serverCommunicator, contentServer);


            _client1 = new(clientCommunicator1, new MockContentClient(), new MockScreenShareClient());

            ConnectionDetails connectionDetails = _server.ConnectionDetails;
            if (!_client1.ConnectToServer(connectionDetails.IpAddress, connectionDetails.Port, "Name1", "1@gmail.com", "photo1"))
            {
                Assert.Fail("Client 1 was not able to connect to the server");
            }
            if (!_client1.ConnectToServer(connectionDetails.IpAddress, connectionDetails.Port, "Name1", "1@gmail.com", "photo1"))
            {
                Assert.Fail("Client 1 was not able to connect to the server");
            }

        }
    }
}
