/******************************************************************************
* Filename    = DashboardIntegrationTests.cs
*
* Author      = Pratham Ravindra Nagpure 
*
* Roll number = 112001054
*
* Product     = Messenger 
* 
* Project     = MessengerTests
*
* Description = A class to test dashboard using mock objects.
*****************************************************************************/

using MessengerDashboard.Server;
using MessengerDashboard.Client;
using MessengerTests.DashboardTests.Mocks;
using MessengerDashboard;
using MessengerContent.DataModels;

namespace MessengerTests.DashboardTests
{

    [TestClass]
    public class DashboardMockObjectsIntegrationTests
    {
        ServerSessionController _server;
        ClientSessionController _client1;
        ClientSessionController _client2;
        int _count = 0;

        [TestMethod]
        public void FullSessionTest()
        {
            MockServerCommunicator serverCommunicator = new ();
            MockClientCommunicator clientCommunicator1 = new ();
            MockClientCommunicator clientCommunicator2 = new ();
            serverCommunicator.AddClientCommunicator("1", clientCommunicator1);
            serverCommunicator.AddClientCommunicator("2", clientCommunicator2);
            clientCommunicator1.SetServer(serverCommunicator);
            clientCommunicator2.SetServer(serverCommunicator);
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
            _server.SessionUpdated += HandleServerSessionUpdated;

            _client1 = new (clientCommunicator1, new MockContentClient(),new MockScreenShareClient());
            _client2 = new (clientCommunicator2,new MockContentClient() , new MockScreenShareClient());

            _client1.Refreshed += HandleClient1Refreshed;
            _client2.Refreshed += HandleClient2Refreshed;

            _client1.SessionChanged += HandleClient1SessionChanged;
            _client2.SessionChanged += HandleClient2SessionChanged;

            _client1.SessionModeChanged += HandleClient1SessionModeChanged;
            _client2.SessionModeChanged += HandleClient2SessionModeChanged;

            _client1.SessionExited += HandleClient1SessionExited;
            _client2.SessionExited += HandleClient2SessionExited;

            ConnectionDetails connectionDetails = _server.ConnectionDetails;
            if (!_client1.ConnectToServer(connectionDetails.IpAddress, connectionDetails.Port, "Name1", "1@gmail.com", "photo1"))
            {
                Assert.Fail("Client 1 was not able to connect to the server");
            }
            if (!_client2.ConnectToServer(connectionDetails.IpAddress, connectionDetails.Port, "Name2", "2@gmail.com", "photo2"))
            {
                Assert.Fail("Client 2 was not able to connect to the server");
            }
            Thread.Sleep(500);
            List<UserInfo> users1 = _client1.SessionInfo.Users;
            List<UserInfo> users2 = _client2.SessionInfo.Users;
            Assert.AreEqual(users1.Count, users2.Count);
            Assert.AreEqual(users1.Count, 2);
            List<UserInfo> correctUsers = new()
            { 
                new("Name1", 1, "1@gmail.com", "photo1"),
                new("Name2", 2, "2@gmail.com", "photo2") 
            };
            if (!AreUserListsEqual(correctUsers, users1) || !AreUserListsEqual(users1, users2))
            {
                Assert.Fail("User data is not correct");
            }
            _client1.SendExamModeRequestToServer();
            Thread.Sleep(500);
            Assert.AreEqual(_client1.SessionInfo.SessionMode, _client2.SessionInfo.SessionMode);
            Assert.AreEqual(_client1.SessionInfo.SessionMode, SessionMode.Exam);
            _client1.SendLabModeRequestToServer();
            Thread.Sleep(500);
            Assert.AreEqual(_client1.SessionInfo.SessionMode, _client2.SessionInfo.SessionMode);
            Assert.AreEqual(_client1.SessionInfo.SessionMode, SessionMode.Lab);
            contentServer.SetChats(chatThreads);
            _client1.SendRefreshRequestToServer();
            _client2.SendRefreshRequestToServer();
            if (!AreSessionEqual(_client1.SessionInfo, _client2.SessionInfo))
            {
                Assert.Fail("Session are not equal");
            }
            _client2.SendExitSessionRequestToServer();
            Thread.Sleep(500);
            _client1.SendExitSessionRequestToServer();
        }

        private void HandleClient1SessionExited(object? sender, MessengerDashboard.Client.Events.SessionExitedEventArgs e)
        {
            Assert.IsNotNull(e);
        }

        private void HandleClient2SessionExited(object? sender, MessengerDashboard.Client.Events.SessionExitedEventArgs e)
        {
            Assert.IsNotNull(e);
        }

        private void HandleClient1SessionModeChanged(object? sender, MessengerDashboard.Client.Events.SessionModeChangedEventArgs e)
        {
            Assert.AreEqual(_client1.SessionInfo.SessionMode, e.SessionMode);
        }

        private void HandleClient2SessionModeChanged(object? sender, MessengerDashboard.Client.Events.SessionModeChangedEventArgs e)
        {
            Assert.AreEqual(_client2.SessionInfo.SessionMode, e.SessionMode);
        }

        private void HandleClient1SessionChanged(object? sender, MessengerDashboard.Client.Events.ClientSessionChangedEventArgs e)
        {
            if (!AreSessionEqual(_client1.SessionInfo, e.Session))
            {
                Assert.Fail("Session not same in Client 1 session changed event.");
            }
        }

        private void HandleClient2SessionChanged(object? sender, MessengerDashboard.Client.Events.ClientSessionChangedEventArgs e)
        {
            if (!AreSessionEqual(_client2.SessionInfo, e.Session))
            {
                Assert.Fail("Session not same in Client 1 session changed event.");
            }
        }

        private void HandleClient1Refreshed(object? sender, MessengerDashboard.Client.Events.RefreshedEventArgs e)
        {
            Assert.IsNotNull(e);
        }

        private void HandleClient2Refreshed(object? sender, MessengerDashboard.Client.Events.RefreshedEventArgs e)
        {
            Assert.IsNotNull(e);
        }


        private void HandleServerSessionUpdated(object? sender, MessengerDashboard.Server.Events.SessionUpdatedEventArgs e)
        {
            Assert.IsNotNull(e);
        }

        private bool AreEqualUsers(UserInfo userInfo1, UserInfo userInfo2)
        {
            return userInfo1.UserId == userInfo2.UserId &&
                    userInfo1.UserName == userInfo2.UserName &&
                    userInfo1.UserEmail == userInfo2.UserEmail &&
                    userInfo2.UserPhotoUrl == userInfo2.UserPhotoUrl;
        }

        private bool AreUserListsEqual(List<UserInfo> list1, List<UserInfo> list2)
        {
            if (list1.Count != list2.Count)
            {
                return false;
            }
            for (int i=0; i<list1.Count; ++i)
            {
                if (!AreEqualUsers(list1[i], list2[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool AreSessionEqual(SessionInfo s1, SessionInfo s2)
        {
           return s1.SessionMode == s2.SessionMode && AreUserListsEqual(s1.Users, s2.Users);
        }

        [TestMethod]
        public void EdgeCasesTests()
        {
            try
            {
                MockServerCommunicator serverCommunicator = new ();
                MockClientCommunicator clientCommunicator1 = new ();
                ClientSessionController clientSessionController = new(clientCommunicator1, new MockContentClient(), new MockScreenShareClient());
                serverCommunicator.AddClient("1", new());
                serverCommunicator.AddClientCommunicator("Dashboard", clientCommunicator1);
                _server = new(serverCommunicator, new MockContentServer());
                clientCommunicator1.SetServer(serverCommunicator);
                clientCommunicator1.Send("", "Dashboard", null);
                ClientPayload clientPayload = new(Operation.GiveUserDetails, null);
                Serializer serializer = new();
                clientCommunicator1.Send(serializer.Serialize(clientPayload), "Dashboard", null);
                clientCommunicator1.Send(null, "Dashboard", null);
                clientCommunicator1.Send("fake", "Dashboard", null);
                clientPayload = new(Operation.TakeUserDetails, new());
                clientCommunicator1.Send(serializer.Serialize(clientPayload), "Dashboard", null);
                clientPayload = new(Operation.TakeUserDetails, new("",1,"",""));
                clientCommunicator1.Send(serializer.Serialize(clientPayload), "Dashboard", null);
                clientPayload = new(Operation.RemoveClient, new("a",-1,"a","a"));
                clientCommunicator1.Send(serializer.Serialize(clientPayload), "Dashboard", null);
                clientPayload = new(Operation.GiveUserDetails, new("a",1,"a","a"));
                clientCommunicator1.Send(serializer.Serialize(clientPayload), "Dashboard", null);
                clientSessionController.SessionExited += ClientSessionController_SessionExited;
                serverCommunicator.OnClientLeft("1");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            Assert.AreEqual(_count, 1);
        }

        private void ClientSessionController_SessionExited(object? sender, MessengerDashboard.Client.Events.SessionExitedEventArgs e)
        {
            _count++;
        }
    }
}
