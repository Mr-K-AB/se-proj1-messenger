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
        /// Tests the successful registration of the clients when clients stop screen sharing.
        /// </summary>
        [TestMethod]
        public void TestDeregisterClient()
        {
            // Arrange.
            // Create view model and mock communicator.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance(isDebugging: true);
            ScreenshareServer server = viewModel.GetPrivate<ScreenshareServer>("_model");
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            int numClients = 2;

            // Create mock clients.
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            List<int> clientIds = clients.Select(client => client.Id).ToList();

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
            viewModel.OnPin(pinnedClient1.Id);
            viewModel.OnPin(pinnedClient2.Id);

            viewModel.OnUnpin(pinnedClient1.Id);
            viewModel.OnUnpin(pinnedClient2.Id);
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
