/******************************************************************************
 * 
 * Author      = M Anish Goud
 *
 * Roll no     = 112001020
 *
 *****************************************************************************/
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
        [TestMethod]
        public void TestForRegisterClient()
        {
            int noOfClients = 3;
            var viewmodelMock = new Mock<IDataReceiver>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
           
            List<SharedClientScreen> clients = Utils.GetMockClients(server, noOfClients, isDebugging: true);

            foreach (SharedClientScreen client in clients)
            {
                DataPacket packet = new(client.Id, client.Name, nameof(ClientDataHeader.Register), "");
                 
                server.OnDataReceived(JsonSerializer.Serialize<DataPacket>(packet));
            }

            foreach (SharedClientScreen client in clients)
            {
                DataPacket packet = new(client.Id, client.Name, nameof(ClientDataHeader.Register), "");

                server.OnDataReceived(JsonSerializer.Serialize<DataPacket>(packet));
            }
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<int, SharedClientScreen>>("_subscribers").Values.ToList();

           
            Assert.AreEqual(noOfClients, subscribers.Count);
            
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
            
            var viewmodelMock = new Mock<IDataReceiver>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);

            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }
            foreach (SharedClientScreen client in clients)
            {
                DataPacket packet = new(client.Id, client.Name, nameof(ClientDataHeader.Deregister), "");
                server.OnDataReceived(JsonSerializer.Serialize<DataPacket>(packet));
            }
            foreach (SharedClientScreen client in clients)
            {
                DataPacket packet = new(client.Id, client.Name, nameof(ClientDataHeader.Deregister), "");
                server.OnDataReceived(JsonSerializer.Serialize<DataPacket>(packet));
            }
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<int, SharedClientScreen>>("_subscribers").Values.ToList();
            Assert.IsTrue(subscribers.Count == 0);
           
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
          
            var viewmodelMock = new Mock<IDataReceiver>();
            int numImages = 5;
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);
            List<SharedClientScreen> clients = Utils.GetMockClients(server,1, isDebugging: true);

            List<(string MockImagePacket, string MockImage)> clientImagePackets = new();
            for (int i = 0; i < numImages; ++i)
            {
                clientImagePackets.Add(Utils.GetMockImagePacket(client.Id, client.Name));
            }
            clientImagePackets.Add(Utils.GetMockImagePacket(clients[0].Id, clients[0].Name));
            string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
            server.OnDataReceived(mockRegisterPacket);
            for (int i = 0; i < 6; ++i)
            {
                server.OnDataReceived(clientImagePackets[i].MockImagePacket);
            }
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<int, SharedClientScreen>>("_subscribers").Values.ToList();
            Trace.WriteLine($"{subscribers.Count}******");
            Assert.IsTrue(subscribers.Count == 4);

            client.Dispose();
            server.Dispose();
            subscribers.Clear();
        }

        [TestMethod]
        public void TestInvalidPacket()
        {
            var viewmodelMock = new Mock<IDataReceiver>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            string packet = null;
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

            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            List<int> clientIds = clients.Select(client => client.Id).ToList();

            (int Rows, int Cols) numRowsColumns = (1, 2);
            server.BroadcastClients(clientIds, "Hello", numRowsColumns);
            server.BroadcastClients(clientIds, nameof(ServerDataHeader.Stop), numRowsColumns);
            foreach(int client in clientIds)
            {
                communicatorMock.Verify(communicator =>
                communicator.Send(It.IsAny<string>(), SSUtils.ClientIdentifier, client.ToString()),
                Times.Exactly(1));
            }

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
        public void TestConfirmationPacket()
        {
            // Arrange.
            int timeOfArrival = 19000;

            var viewmodelMock = new Mock<IDataReceiver>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);

            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            Thread.Sleep(timeOfArrival);

          
            foreach (SharedClientScreen client in clients)
            {
                string mockConfirmationPacket = Utils.GetMockConfirmationPacket(client.Id, client.Name);
                server.OnDataReceived(mockConfirmationPacket);
            }
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<int, SharedClientScreen>>("_subscribers").Values.ToList();
            Trace.WriteLine($"{subscribers.Count}");
            Assert.IsTrue(subscribers.Count == numClients);
            foreach (SharedClientScreen client in clients)
            {
                Assert.IsTrue(subscribers.FindIndex(c => c.Id == client.Id) != -1);
            }
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            server.Dispose();
            subscribers.Clear();
        }
        
        [TestMethod]

        public void TestTimeout()
        {
            int timeOfArrival = 21000;
            var viewmodelMock = new Mock<IDataReceiver>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);

            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }
            Thread.Sleep(timeOfArrival);

            foreach (SharedClientScreen client in clients)
            {
                string mockConfirmationPacket = Utils.GetMockConfirmationPacket(client.Id, client.Name);
                server.OnDataReceived(mockConfirmationPacket);
            }
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<int, SharedClientScreen>>("_subscribers").Values.ToList();

            Assert.IsTrue(subscribers.Count == 0);

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
