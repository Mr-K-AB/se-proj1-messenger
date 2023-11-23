using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class ClientTest1
    {
        ServerSessionController _server;
        ClientSessionController _client1;
        ClientSessionController _client;

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
            if (_client1.ConnectToServer(connectionDetails.IpAddress, connectionDetails.Port, "", "1@gmail.com", "photo1"))
            {
                Assert.Fail("Client 1 got connect which is wrong");
            }
        }

        [TestMethod]
        public void Test2()
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


    }
}
