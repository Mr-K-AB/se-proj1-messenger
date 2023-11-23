/************************************************************************
* Filename    = ScreenshareClientViewModelTest.cs
*
* Author      = Likhith Reddy
*
* Product     = ScreenShare
* 
* Project     = Messenger
*
* Description = Contains test for view model for screenshare client.
************************************************************************/

using MessengerScreenshare.Client;
using MessengerScreenshare;
using System.Text.Json;
using System.Windows.Threading;

namespace MessengerTests.ScreenshareTests
{
    [TestClass]
    public class ScreenshareClientViewModelTests
    {
        [TestMethod]
        public void TestSetTrueSharingScreen()
        {
            ScreenshareClientViewModel screenshareClientViewModel = new();
            Assert.IsFalse(screenshareClientViewModel.SharingScreen);

            ScreenshareClient model = screenshareClientViewModel.GetPrivate<ScreenshareClient>("_model");
            model.SetUser(-1, "test");
            screenshareClientViewModel.SharingScreen = true;

            DispatcherOperation? sharingScreenOp = screenshareClientViewModel.GetPrivate<DispatcherOperation?>("_sharingScreenOp");
            sharingScreenOp!.Wait();
            Assert.IsTrue(screenshareClientViewModel.SharingScreen);
            Assert.IsNotNull(sharingScreenOp);
        }

        [TestMethod]
        public void TestSetFalseSharingScreen()
        {
            ScreenshareClientViewModel screenshareClientViewModel = new();
            Assert.IsFalse(screenshareClientViewModel.SharingScreen);

            ScreenshareClient model = screenshareClientViewModel.GetPrivate<ScreenshareClient>("_model");
            model.SetUser(-1, "test");
            screenshareClientViewModel.SharingScreen = true;
            DispatcherOperation? sharingScreenOp = screenshareClientViewModel.GetPrivate<DispatcherOperation?>("_sharingScreenOp");
            sharingScreenOp!.Wait();

            DataPacket packet = new(-1, "test", ServerDataHeader.Send.ToString(), "10");
            string serializedData = JsonSerializer.Serialize(packet);
            model.OnDataReceived(serializedData);

            screenshareClientViewModel.SharingScreen = false;
            sharingScreenOp = screenshareClientViewModel.GetPrivate<DispatcherOperation?>("_sharingScreenOp");
            sharingScreenOp!.Wait();
            Assert.IsFalse(screenshareClientViewModel.SharingScreen);
            Assert.IsNotNull(sharingScreenOp);
        }
    }
}
