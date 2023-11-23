/*******************************************************************************************
* Filename    = ScreenshareServerViewModelTest.cs
*
* Author      = Likhith Reddy
*
* Product     = ScreenShare
* 
* Project     = Messenger
*
* Description = Contains tests for methods defined in "ScreenshareServerViewModel" class.
********************************************************************************************/

using Moq;
using MessengerNetworking.Communicator;
using MessengerScreenshare.Server;
using System.ComponentModel;
using System.Windows.Threading;
using SSUtils = MessengerScreenshare.Utils;
using Timer = System.Timers.Timer;

namespace MessengerTests.ScreenshareTests
{
    /// <summary>
    /// Contains tests for methods defined in "ScreenshareServerViewModel" class.
    /// </summary>
    [TestClass]
    public class ScreenshareServerViewModelTests
    {
        /// <summary>
        /// Tests the successful registration of the clients when clients start screen sharing.
        /// </summary>
        /// <param name="numClients">
        /// Number of clients who registered.
        /// </param>
        [TestMethod]
        public void TestRegisterClient()
        {
            // Arrange.
            // Create view model and mock communicator.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance(isDebugging: true);
            ScreenshareServer server = viewModel.GetPrivate<ScreenshareServer>("_model");
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            int numClients = 3;

            // Create mock clients.
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            List<int> clientIds = clients.Select(client => client.Id).ToList();

            // Add custom handler to the PropertyChanged event.
            int invokedCount = 0;
            PropertyChangedEventHandler handler = new((_, _) => ++invokedCount);
            viewModel.PropertyChanged += handler;

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Disable timers for the clients for testing.
            DisableTimer(server.GetPrivate<Dictionary<int, SharedClientScreen>>("_subscribers").Values.ToList());

            // Get the main thread dispatcher operations from "BeginInvoke".
            DispatcherOperation? updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            DispatcherOperation? displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            // Assert that they are being set.
            Assert.IsTrue(updateViewOp != null);
            Assert.IsTrue(displayPopupOp != null);

            // Wait for the operations to finish.
            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers = viewModel.GetPrivate<List<SharedClientScreen>>("_subscribers");
            List<SharedClientScreen> currentClients = viewModel.CurrentClients.ToList();

            // Check that each client registered is still present.
            Assert.IsTrue(subscribers.Count == numClients);

            // Check that all the fields for the clients are properly set.
            foreach (SharedClientScreen client in clients)
            {
                int clientIdx = subscribers.FindIndex(c => c.Id == client.Id);
                Assert.IsTrue(clientIdx != -1);
                Assert.IsTrue(subscribers[clientIdx].Name == client.Name);
                Assert.IsTrue(!subscribers[clientIdx].Pinned);
                Assert.IsTrue(subscribers[clientIdx].TileHeight >= 0);
                Assert.IsTrue(subscribers[clientIdx].TileWidth >= 0);
            }

            // Check that the subscribers are stored in sorted order.
            Assert.IsTrue(!subscribers.Zip(subscribers.Skip(1), (a, b) => string.Compare(a.Name, b.Name) != 1).Contains(false));

            // Check various fields of view model based on initial view rendering logic.
            Assert.IsTrue(currentClients.Count == Math.Min(ScreenshareServerViewModel.MaxTiles, numClients));
            Assert.IsTrue(viewModel.CurrentRows >= 0);
            Assert.IsTrue(viewModel.CurrentColumns >= 0);
            Assert.IsTrue(viewModel.IsPopupOpen == true);
            Assert.IsTrue(viewModel.PopupText != "");

            // Check that custom handler was invoked at least as many times the number of clients registered.
            Assert.IsTrue(invokedCount > 0);

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            viewModel.PropertyChanged -= handler;
            viewModel.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful registration of the clients when clients stop screen sharing.
        /// </summary>
        /// <param name="numClients">
        /// Number of clients who registered.
        /// </param>
        [TestMethod]
        public void TestDeregisterClient()
        {
            // Arrange.
            // Create view model and mock communicator.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance(isDebugging: true);
            ScreenshareServer server = viewModel.GetPrivate<ScreenshareServer>("_model");
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            int numClients = 3;

            // Create mock clients.
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            List<int> clientIds = clients.Select(client => client.Id).ToList();

            // Add custom handler to the PropertyChanged event.
            int invokedCount = 0;
            PropertyChangedEventHandler handler = new((_, _) => ++invokedCount);
            viewModel.PropertyChanged += handler;

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Disable timers for the clients for testing.
            DisableTimer(server.GetPrivate<Dictionary<int, SharedClientScreen>>("_subscribers").Values.ToList());

            // Deregister the clients by sending mock deregister packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockDeregisterPacket = Utils.GetMockDeregisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockDeregisterPacket);
            }

            // Get the main thread dispatcher operations from "BeginInvoke".
            DispatcherOperation? updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            DispatcherOperation? displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            // Assert that they are being set.
            Assert.IsTrue(updateViewOp != null);
            Assert.IsTrue(displayPopupOp != null);

            // Wait for the operations to finish.
            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers = viewModel.GetPrivate<List<SharedClientScreen>>("_subscribers");
            List<SharedClientScreen> currentClients = viewModel.CurrentClients.ToList();

            // Check that each client registered is de-registered.
            Assert.IsTrue(subscribers.Count == 0);
            Assert.IsTrue(currentClients.Count == 0);

            // Check various fields of view model based on initial view rendering logic.
            Assert.IsTrue(viewModel.IsPopupOpen == true);
            Assert.IsTrue(viewModel.PopupText != "");

            // Check that custom handler was invoked at least as many times the number of clients registered.
            Assert.IsTrue(invokedCount > 0);

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            viewModel.PropertyChanged -= handler;
            viewModel.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful marking of a single client as pinned.
        /// </summary>
        /// <param name="numClients">
        /// Number of clients who registered.
        /// </param>
        [TestMethod]
        public void TestOnPinSingle()
        {
            int numClients = 1;

            // Arrange.
            // Create view model and mock communicator.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance(isDebugging: true);
            ScreenshareServer server = viewModel.GetPrivate<ScreenshareServer>("_model");
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            // Create mock clients.
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            List<int> clientIds = clients.Select(client => client.Id).ToList();

            // Choose a client to be marked as pinned.
            SharedClientScreen pinnedClient = clients[^1];

            // Add custom handler to the PropertyChanged event.
            int invokedCount = 0;
            PropertyChangedEventHandler handler = new((_, _) => ++invokedCount);
            viewModel.PropertyChanged += handler;

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Disable timers for the clients for testing.
            DisableTimer(server.GetPrivate<Dictionary<int, SharedClientScreen>>("_subscribers").Values.ToList());

            // Get the main thread dispatcher operations from "BeginInvoke".
            DispatcherOperation? updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            DispatcherOperation? displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            // Assert that they are being set.
            Assert.IsTrue(updateViewOp != null);
            Assert.IsTrue(displayPopupOp != null);

            // Wait for the operations to finish.
            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Mark the client as pinned.
            viewModel.OnPin(pinnedClient.Id);

            // Get the updated operations.
            updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            // Assert that they are being set.
            Assert.IsTrue(updateViewOp != null);
            Assert.IsTrue(displayPopupOp != null);

            // Wait for the operations to finish.
            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers = viewModel.GetPrivate<List<SharedClientScreen>>("_subscribers");
            List<SharedClientScreen> currentClients = viewModel.CurrentClients.ToList();

            // Check that each client registered is still present.
            Assert.IsTrue(subscribers.Count == numClients);
            foreach (SharedClientScreen client in clients)
            {
                Assert.IsTrue(subscribers.FindIndex(c => c.Id == client.Id) != -1);
            }

            // Check that the first client in the reordered subscribers list is the pinned client.
            Assert.IsTrue(subscribers[0].Id == pinnedClient.Id);
            Assert.IsTrue(subscribers[0].Pinned);

            // Check various fields of view model based on view rendering logic.
            Assert.IsTrue(viewModel.IsPopupOpen == true);
            Assert.IsTrue(viewModel.PopupText != "");

            // Check that custom handler was invoked at least as many times the number of clients registered.
            Assert.IsTrue(invokedCount > 0);

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            viewModel.PropertyChanged -= handler;
            viewModel.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful marking of multiple clients as pinned.
        /// </summary>
        /// <param name="numClients">
        /// Number o
        [TestMethod]
        public void TestOnPinMultiple()
        {
            int numClients = 2;

            // Arrange.
            // Create view model and mock communicator.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance(isDebugging: true);
            ScreenshareServer server = viewModel.GetPrivate<ScreenshareServer>("_model");
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            // Create mock clients.
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            clients = clients.OrderBy(subscriber => subscriber.Name).ToList();
            List<int> clientIds = clients.Select(client => client.Id).ToList();

            // Choose any two clients to be marked as pinned.
            SharedClientScreen pinnedClient1 = clients[0], pinnedClient2 = clients[^1];

            // Add custom handler to the PropertyChanged event.
            int invokedCount = 0;
            PropertyChangedEventHandler handler = new((_, _) => ++invokedCount);
            viewModel.PropertyChanged += handler;

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Disable timers for the clients for testing.
            DisableTimer(server.GetPrivate<Dictionary<int, SharedClientScreen>>("_subscribers").Values.ToList());

            // Get the main thread dispatcher operations from "BeginInvoke".
            DispatcherOperation? updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            DispatcherOperation? displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            // Assert that they are being set.
            Assert.IsTrue(updateViewOp != null);
            Assert.IsTrue(displayPopupOp != null);

            // Wait for the operations to finish.
            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Mark the clients as pinned and wait for their operations to finish.
            viewModel.OnPin(pinnedClient1.Id);

            updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            Assert.IsTrue(updateViewOp != null);
            Assert.IsTrue(displayPopupOp != null);

            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            viewModel.OnPin(pinnedClient2.Id);

            updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            Assert.IsTrue(updateViewOp != null);
            Assert.IsTrue(displayPopupOp != null);

            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers = viewModel.GetPrivate<List<SharedClientScreen>>("_subscribers");
            List<SharedClientScreen> currentClients = viewModel.CurrentClients.ToList();

            // Check that each client registered is still present.
            Assert.IsTrue(subscribers.Count == numClients);
            foreach (SharedClientScreen client in clients)
            {
                Assert.IsTrue(subscribers.FindIndex(c => c.Id == client.Id) != -1);
            }

            // Check that the first two clients in the reordered subscribers list are the pinned clients.
            Assert.IsTrue(subscribers[0].Id == pinnedClient1.Id);
            Assert.IsTrue(subscribers[1].Id == pinnedClient2.Id);
            Assert.IsTrue(subscribers[0].Pinned);
            Assert.IsTrue(subscribers[1].Pinned);

            // Check various fields of view model based on view rendering logic.
            Assert.IsTrue(viewModel.IsPopupOpen == true);
            Assert.IsTrue(viewModel.PopupText != "");

            // Check that custom handler was invoked at least as many times the number of clients registered.
            Assert.IsTrue(invokedCount > 0);

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            viewModel.PropertyChanged -= handler;
            viewModel.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful marking of a client as unpinned.
        /// </summary>
        /// <param name="numClients">
        /// Number of clients who registered.
        /// </param>
        [TestMethod]
        public void TestOnUnpin()
        {
            // Arrange.
            // Create view model and mock communicator.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance(isDebugging: true);
            ScreenshareServer server = viewModel.GetPrivate<ScreenshareServer>("_model");
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            int numClients = 1;

            // Create mock clients.
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            List<int> clientIds = clients.Select(client => client.Id).ToList();

            // Choose a client to be marked as pinned.
            SharedClientScreen pinnedClient = clients[^1];

            // Add custom handler to the PropertyChanged event.
            int invokedCount = 0;
            PropertyChangedEventHandler handler = new((_, _) => ++invokedCount);
            viewModel.PropertyChanged += handler;

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Disable timers for the clients for testing.
            DisableTimer(server.GetPrivate<Dictionary<int, SharedClientScreen>>("_subscribers").Values.ToList());

            // Get the main thread dispatcher operations from "BeginInvoke".
            DispatcherOperation? updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            DispatcherOperation? displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            // Assert that they are being set.
            Assert.IsTrue(updateViewOp != null);
            Assert.IsTrue(displayPopupOp != null);

            // Wait for the operations to finish.
            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Mark the client as pinned and wait for their operations to be finished.
            viewModel.OnPin(pinnedClient.Id);

            updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            Assert.IsTrue(updateViewOp != null);
            Assert.IsTrue(displayPopupOp != null);

            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Mark the client as unpinned and wait for their operations to be finished.
            viewModel.OnUnpin(pinnedClient.Id);

            updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            Assert.IsTrue(updateViewOp != null);
            Assert.IsTrue(displayPopupOp != null);

            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers = viewModel.GetPrivate<List<SharedClientScreen>>("_subscribers");
            List<SharedClientScreen> currentClients = viewModel.CurrentClients.ToList();

            // Check that each client registered is still present.
            Assert.IsTrue(subscribers.Count == numClients);
            foreach (SharedClientScreen client in clients)
            {
                int clientIdx = subscribers.FindIndex(c => c.Id == client.Id);
                Assert.IsTrue(clientIdx != -1);
                if (client.Id == pinnedClient.Id)
                {
                    // Assert that the pinned client is successfully unpinned.
                    Assert.IsTrue(!subscribers[clientIdx].Pinned);
                }
            }

            // Check that the subscribers are stored in sorted order.
            Assert.IsTrue(!subscribers.Zip(subscribers.Skip(1), (a, b) => string.Compare(a.Name, b.Name) != 1).Contains(false));

            // Check various fields of view model based on view rendering logic.
            Assert.IsTrue(currentClients.Count == Math.Min(ScreenshareServerViewModel.MaxTiles, numClients));
            Assert.IsTrue(viewModel.IsPopupOpen == true);
            Assert.IsTrue(viewModel.PopupText != "");

            // Check that custom handler was invoked at least as many times the number of clients registered.
            Assert.IsTrue(invokedCount > 0);

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            viewModel.PropertyChanged -= handler;
            viewModel.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful disposal of the view model.
        /// </summary>
        [TestMethod]
        public void TestDispose()
        {
            // Arrange.
            // Create view model and mock communicator.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance(isDebugging: true);
            ScreenshareServer server = viewModel.GetPrivate<ScreenshareServer>("_model");
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            // Create mock clients.
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Dispose of the view model.
            viewModel.Dispose();
            // Try disposing again.
            viewModel.Dispose();

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers = viewModel.GetPrivate<List<SharedClientScreen>>("_subscribers");

            // Check that each client registered is de-registered.
            Assert.IsTrue(subscribers.Count == 0);

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            subscribers.Clear();
        }

        /// <summary>
        /// Helper method to disable the timer for the clients on the server to prevent
        /// timeout from occurring during testing.
        /// </summary>
        /// <param name="subscribers">
        /// List of subscribers whose timeout is to be disabled.
        /// </param>
        private static void DisableTimer(List<SharedClientScreen> subscribers)
        {
            foreach (SharedClientScreen clientScreen in subscribers)
            {
                clientScreen.GetPrivate<Timer>("_timer").Enabled = false;
            }
        }
    }
}
