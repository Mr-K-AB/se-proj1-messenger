using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerContent.DataModels;
using MessengerDashboard;
using MessengerDashboard.Client;
using MessengerDashboard.Server;
using MessengerTests.DashboardTests.Mocks;

namespace MessengerTests.DashboardTests
{
    [TestClass]
    public class ClientTest
    {
        ServerSessionController _server;
        ClientSessionController _client1;

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
