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
using MessengerContent.DataModels;

namespace MessengerTests.DashboardTests
{
    [TestClass]
    public class SessionTestsWIthCommunicator
    {
        [TestMethod]
        public void SessionJoinAndLeaveTest()
        {

            IServerSessionController server = DashboardFactory.GetServerSessionController();
            IClientSessionController client = DashboardFactory.GetClientSessionController();
            client.ConnectToServer(server.ConnectionDetails.IpAddress, server.ConnectionDetails.Port, "a", "a", "a");
            Assert.IsTrue(client.IsConnectedToServer);
            client.SendExitSessionRequestToServer();
            Assert.IsFalse(client.IsConnectedToServer);
        }
    }
}
