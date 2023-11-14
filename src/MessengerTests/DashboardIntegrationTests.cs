using System.Diagnostics;
using System.Xml.Serialization;
using MessengerDashboard.Summarization;
using MessengerDashboard;
using MessengerDashboard.Sentiment;
using MessengerNetworking.Communicator;
using MessengerNetworking.Factory;
using MessengerDashboard.Server;
using MessengerDashboard.Client;
using MessengerContent.Client;
using MessengerContent.Server;
using MessengerContent.DataModels;
using MessengerContent;

namespace MessengerTests
{
    [TestClass]
    public class DashboardIntegrationTests
    {
        [TestMethod]
        public void SessionTest()
        {
            ICommunicator communicator = Factory.GetInstance();
            ServerSessionController server = new(communicator);
            ConnectionDetails connectionDetails = server.ConnectionDetails;
            ClientSessionController client = new(new UdpCommunicator());
            if (!client.ConnectToServer(connectionDetails.IpAddress, connectionDetails.Port, 5000, "helo", "heko", "helo"))
            {
                Assert.Fail();
            }
        }

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
                    if (!client.RequestServerToRemoveClient(1000))
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
    }
}
