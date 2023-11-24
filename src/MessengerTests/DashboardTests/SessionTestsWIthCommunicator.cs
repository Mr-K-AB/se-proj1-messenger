/******************************************************************************
* Filename    = SessionTestsWIthCommunicator.cs
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
using MessengerContent.DataModels;

namespace MessengerTests.DashboardTests
{
    [TestClass]
    public class SessionTestsWIthCommunicator
    {
        private IClientSessionController _client;

        [TestMethod]
        public void SessionJoinAndLeaveTest()
        {

            IServerSessionController server = DashboardFactory.GetServerSessionController();
            _client = DashboardFactory.GetClientSessionController();
            _client.ConnectToServer(server.ConnectionDetails.IpAddress, server.ConnectionDetails.Port, "a", "a", "a");
            Assert.IsTrue(_client.IsConnectedToServer);
            _client.SendExitSessionRequestToServer();
            _client.SessionExited += HandleSessionExited;
        }

        private void HandleSessionExited(object? sender, MessengerDashboard.Client.Events.SessionExitedEventArgs e)
        {
            Assert.IsFalse(_client.IsConnectedToServer);
        }
    }
}
