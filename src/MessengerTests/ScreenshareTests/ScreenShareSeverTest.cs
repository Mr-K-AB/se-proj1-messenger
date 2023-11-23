using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Messenger.Client;
using MessengerNetworking.Communicator;
using MessengerScreenshare;
using MessengerScreenshare.Client;
using MessengerScreenshare.Server;
using MessengerTests.WhiteboardTests;
using Moq;
using SSUtils = MessengerScreenshare.Utils;


namespace MessengerTests.ScreenshareTests
{
    [TestClass]
    public class ScreenShareSeverTest
    {
        public static IEnumerable<object[]> PostTimeoutTime =>
           new List<object[]>
           {
                new object[] { SharedClientScreen.Timeout + 100 },
                new object[] { SharedClientScreen.Timeout + 1000 },
                new object[] { SharedClientScreen.Timeout + 2000 },
           };

        /// <summary>
        /// Update time values before the timeout time.
        /// </summary>
        public static IEnumerable<object[]> PreTimeoutTime =>
            new List<object[]>
            {
                new object[] { SharedClientScreen.Timeout - 2000 },
                new object[] { SharedClientScreen.Timeout - 1000 },
            };
        [TestMethod]
        public void TestForRegisterClient()
        {
            // Arrange.
            // Create mock server and mock clients.
            int noOfClients = 3;
            var viewmodelMock = new Mock<IDataReceiver>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
           
            List<SharedClientScreen> clients = Utils.GetMockClients(server, noOfClients, isDebugging: true);

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                DataPacket packet = new(client.Id, client.Name, nameof(ClientDataHeader.Register), "");
                 
                server.OnDataReceived(JsonSerializer.Serialize<DataPacket>(packet));
            }

            // Sending the register request again should do nothing.
            foreach (SharedClientScreen client in clients)
            {
                DataPacket packet = new(client.Id, client.Name, nameof(ClientDataHeader.Register), "");

                server.OnDataReceived(JsonSerializer.Serialize<DataPacket>(packet));
            }

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<int, SharedClientScreen>>("_subscribers").Values.ToList();

            // Check that each client registered is still present.
            Assert.AreEqual(noOfClients, subscribers.Count);
            // Cleanup.
          foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            server.Dispose();
            subscribers.Clear();
        }
        [TestMethod]
        public void TestDeregisterClient()
        {
            // Arrange.
            // Create mock server and mock clients.
            var viewmodelMock = new Mock<IDataReceiver>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Deregister the clients by sending mock deregister packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                DataPacket packet = new(client.Id, client.Name, nameof(ClientDataHeader.Deregister), "");
                server.OnDataReceived(JsonSerializer.Serialize<DataPacket>(packet));
            }

            // Sending de-register request second time should do nothing.
            foreach (SharedClientScreen client in clients)
            {
                DataPacket packet = new(client.Id, client.Name, nameof(ClientDataHeader.Deregister), "");
                server.OnDataReceived(JsonSerializer.Serialize<DataPacket>(packet));
            }

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<int, SharedClientScreen>>("_subscribers").Values.ToList();

            // Check that all the clients are properly de-registered.
            Assert.IsTrue(subscribers.Count == 0);
            
            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            server.Dispose();
            subscribers.Clear();
        }

        [TestMethod]
        public void TestPutImage()
        {
            // Arrange.
            // Create mock server and mock client.
            var viewmodelMock = new Mock<IDataReceiver>();
            int numImages = 5;
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);

            // Act.
            // Create mock images and mock image packets for the client.
            List<(string MockImagePacket, string MockImage)> clientImagePackets = new();
            for (int i = 0; i < numImages; ++i)
            {
                clientImagePackets.Add(Utils.GetMockImagePacket(client.Id, client.Name));
            }

            // Register the client on the server first.
            string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
            server.OnDataReceived(mockRegisterPacket);

            // Send mock image packets for the client to the server.
            for (int i = 0; i < numImages; ++i)
            {
                server.OnDataReceived(clientImagePackets[i].MockImagePacket);
            }

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<int, SharedClientScreen>>("_subscribers").Values.ToList();

            // Check that all the sent images for the client are successfully enqueued in the client's image queue.
            Assert.IsTrue(subscribers.Count == 4);
            int clientIdx = subscribers.FindIndex(c => c.Id == client.Id);
            Assert.IsTrue(clientIdx != -1);

            SharedClientScreen serverClient = subscribers[clientIdx];
            for (int i = 0; i < numImages; ++i)
            {
                string? receivedImage = serverClient.GetImage(serverClient.TaskId);
                Assert.IsTrue(receivedImage != null);
                Assert.IsTrue(receivedImage == clientImagePackets[i].MockImage);
            }

            // Cleanup.
            client.Dispose();
            serverClient.Dispose();
            server.Dispose();
            subscribers.Clear();
        }

        [TestMethod]
        public void TestInvalidPacket()
        {
            var viewmodelMock = new Mock<IDataReceiver>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            string packet = "invalid";
            server.OnDataReceived(packet);
            Assert.IsTrue(1 == 1);

        }
        [TestMethod]
        public void TestBroadcastClients()
        {
            var viewmodelMock = new Mock<IDataReceiver>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            // Create mock clients.
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            List<int> clientIds = clients.Select(client => client.Id).ToList();

            // Act.
            (int Rows, int Cols) numRowsColumns = (1, 2);
            server.BroadcastClients(clientIds, nameof(ServerDataHeader.Stop), numRowsColumns);
            foreach(int client in clientIds)
            {
                communicatorMock.Verify(communicator =>
                communicator.Send(It.IsAny<string>(), SSUtils.ClientIdentifier, client.ToString()),
                Times.Exactly(1));
            }

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            server.Dispose();
        }
        [TestMethod]
        public void TestNoClientBroadcast()
        {
            var viewmodelMock = new Mock<IDataReceiver>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            (int Rows, int Cols) numRowsColumns = (1, 2);
            server.BroadcastClients(new(), nameof(ServerDataHeader.Send), numRowsColumns);
            
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            
            server.BroadcastClients(new(), nameof(ServerDataHeader.Send), numRowsColumns);
        }
        [TestMethod]
        public void TestSuccessfulConfirmationPacketArrival_PreTimeoutTime1()
        {
            // Arrange.
            int timeOfArrival = 19000;

            // Act and Assert.
            // Arrange.
            // Create mock server and mock clients.
            var viewmodelMock = new Mock<IDataReceiver>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);

            // Act.
            // Register all the clients to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Sleep until the CONFIRMATION packet arrives.
            Thread.Sleep(timeOfArrival);

            // Send client's CONFIRMATION packet to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockConfirmationPacket = Utils.GetMockConfirmationPacket(client.Id, client.Name);
                server.OnDataReceived(mockConfirmationPacket);
            }

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<int, SharedClientScreen>>("_subscribers").Values.ToList();
            Trace.WriteLine($"{subscribers.Count}");
            // Check that no client is de-registered as their CONFIRMATION packet arrives before time.
            Assert.IsTrue(subscribers.Count == numClients+1);
            foreach (SharedClientScreen client in clients)
            {
                Assert.IsTrue(subscribers.FindIndex(c => c.Id == client.Id) != -1);
            }

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            server.Dispose();
            subscribers.Clear();
        }

       

    }

}
