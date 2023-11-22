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
* Description = A class to test dashboard's integration with other modules.
*****************************************************************************/

using MessengerDashboard.Server;
using MessengerDashboard.Client;
using MessengerTests.DashboardTests.Mocks;
using MessengerDashboard;

namespace MessengerTests.DashboardTests
{
    [TestClass]
    public class DashboardIntegrationTests
    {
        [TestMethod]
        public void SessionJoinAndLeaveTest()
        {
            MockServerCommunicator serverCommunicator = new ();
            MockClientCommunicator clientCommunicator1 = new ();
            MockClientCommunicator clientCommunicator2 = new ();
            serverCommunicator.AddClientCommunicator("1", clientCommunicator1);
            serverCommunicator.AddClientCommunicator("2", clientCommunicator2);
            clientCommunicator1.SetServer(serverCommunicator);
            clientCommunicator2.SetServer(serverCommunicator);
            ServerSessionController server = new (serverCommunicator);
            ConnectionDetails connectionDetails = server.ConnectionDetails;
            ClientSessionController client1 = new (clientCommunicator1);
            ClientSessionController client2 = new (clientCommunicator2);
            if (!client1.ConnectToServer(connectionDetails.IpAddress, connectionDetails.Port, "Name1", "1@gmail.com", "photo1"))
            {
                Assert.Fail("Client 1 was not able to connect to the server");
            }
            if (!client2.ConnectToServer(connectionDetails.IpAddress, connectionDetails.Port, "Name2", "2@gmail.com", "photo2"))
            {
                Assert.Fail("Client 2 was not able to connect to the server");
            }
            Thread.Sleep(500);
            List<UserInfo> users1 = client1.SessionInfo.Users;
            List<UserInfo> users2 = client2.SessionInfo.Users;
            Assert.AreEqual(users1.Count, users2.Count);
            Assert.AreEqual(users1.Count, 2);
            List<UserInfo> correctUsers = new()
            { 
                new("Name1", 1, "1@gmail.com", "photo1"),
                new("Name2", 2, "2@gmail.com", "photo2") 
            };
            if (!AreEqualUsers(users1[0], correctUsers[0]) || !AreEqualUsers(users1[1], correctUsers[1]) ||
                !AreEqualUsers(users2[0], correctUsers[0]) || !AreEqualUsers(users2[1], correctUsers[1]))
            {
                Assert.Fail("User data is not correct");
            }
            client1.SendExamModeRequestToServer();
            Thread.Sleep(500);
            Assert.AreEqual(client1.SessionInfo.SessionMode, client2.SessionInfo.SessionMode);
            Assert.AreEqual(client1.SessionInfo.SessionMode, SessionMode.Exam);
            client1.SendLabModeRequestToServer();
            Thread.Sleep(500);
            Assert.AreEqual(client1.SessionInfo.SessionMode, client2.SessionInfo.SessionMode);
            Assert.AreEqual(client1.SessionInfo.SessionMode, SessionMode.Lab);
        }

        private bool AreEqualUsers(UserInfo userInfo1, UserInfo userInfo2)
        {
            return userInfo1.UserId == userInfo2.UserId &&
                    userInfo1.UserName == userInfo2.UserName &&
                    userInfo1.UserEmail == userInfo2.UserEmail &&
                    userInfo2.UserPhotoUrl == userInfo2.UserPhotoUrl;
        }

        /*
        [TestMethod]
        public void SessionStressTest()
        {
            ICommunicator communicator = Factory.GetInstance();
            ServerSessionController server = new(communicator);
            ConnectionDetails connectionDetails = server.ConnectionDetails;
            List<Thread> threads = new();
            for (int i = 0; i < 50; ++i)
            {
                Thread thread = new(() =>
                {
                    ClientSessionController client = new(new UdpCommunicator());
                    if (!client.ConnectToServer(connectionDetails.IpAddress, connectionDetails.Port, 5000, "helo", "heko", "helo"))
                    {
                        Assert.Fail();
                    }
                });
                thread.Start();
                threads.Add(thread);
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }

        [TestMethod]
        public void ClientJoinAndExit()
        {
            ICommunicator communicator = Factory.GetInstance();
            ServerSessionController server = new(communicator);
            ConnectionDetails connectionDetails = server.ConnectionDetails;
            List<Thread> threads = new();
            for (int i = 0; i < 5; ++i)
            {
                Thread thread = new(() =>
                {
                    ClientSessionController client = new(new UdpCommunicator());
                    if (!client.ConnectToServer(connectionDetails.IpAddress, connectionDetails.Port, 5000, "helo", "heko", "helo"))
                    {
                        Assert.Fail();
                    }
                });
                thread.Start();
                threads.Add(thread);
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }
        */
    }
}
