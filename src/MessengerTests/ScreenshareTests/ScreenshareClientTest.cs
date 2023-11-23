/*using MessengerNetworking.Communicator;
using MessengerScreenshare.Client;
using MessengerScreenshare;
using Moq;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using SSUtils = MessengerScreenshare.Utils;

namespace MessengerTests.ScreenshareTests
{
    [TestClass]
    public class ScreenshareClientTest
    {
        [TestMethod]
        public void TestSingleton()
        {
            ScreenshareClient screenshareClient = ScreenshareClient.GetInstance(isDebugging: true);
            Debug.Assert(screenshareClient != null);
        }

        [TestMethod]
        public void TestRegisterPacketSend()
        {
            ScreenshareClient screenshareClient = ScreenshareClient.GetInstance(isDebugging: true);

            var communicatorMock = new Mock<ICommunicator>();
            screenshareClient.SetPrivate("_communicator", communicatorMock.Object);

            screenshareClient.SetUser(0, "name");
            Task.Run(async() => await screenshareClient.StartScreensharingAsync());

            string argString = "";
            communicatorMock.Setup(p => p.Broadcast(SSUtils.ClientIdentifier, It.IsAny<string>(), 0))
                .Callback((string s, string s2, int s3) => { if (argString == "") { argString = s; } });

            DataPacket? packet = JsonSerializer.Deserialize<DataPacket>(argString);

            Assert.IsTrue(packet?.Header == ClientDataHeader.Register.ToString());
            //communicatorMock.Verify(communicator =>
            //    communicator.Send(It.IsAny<string>(), SSUtils.ModuleIdentifier, null),
            //    Times.AtLeastOnce);
        }

        [TestMethod]
        public void TestSendPacketReceive()
        {
            ScreenshareClient screenshareClient = ScreenshareClient.GetInstance(isDebugging: true);

            var communicatorMock = new Mock<ICommunicator>();
            screenshareClient.SetPrivate("_communicator", communicatorMock.Object);

            screenshareClient.SetUser(0, "name");
            Task.Run(async () => await screenshareClient.StartScreensharingAsync());

            bool isImagePacketSent = false;
            communicatorMock.Setup(p => p.Broadcast(SSUtils.ClientIdentifier, It.IsAny<string>(), 0))
                .Callback((string s, string s2, int s3) =>
                {
                    DataPacket? packet = JsonSerializer.Deserialize<DataPacket>(s);
                    if (packet?.Header == ClientDataHeader.Image.ToString())
                    {
                        isImagePacketSent = true;
                    }
                });

            DataPacket packet = new(0, "name", ServerDataHeader.Send.ToString(), "10");
            string serializedData = JsonSerializer.Serialize(packet);

            screenshareClient.OnDataReceived(serializedData);

            Thread.Sleep(1000);

            Assert.IsTrue(isImagePacketSent);
            screenshareClient.StopScreensharing();
        }

        [TestMethod]
        public void TestStopPacketReceive()
        {
            ScreenshareClient screenshareClient = ScreenshareClient.GetInstance(isDebugging: true);

            var communicatorMock = new Mock<ICommunicator>();
            screenshareClient.SetPrivate("_communicator", communicatorMock.Object);

            screenshareClient.SetUser(0, "name");
            Task.Run(async () => await screenshareClient.StartScreensharingAsync());

            bool isImagePacketSent = false;
            communicatorMock.Setup(p => p.Broadcast(SSUtils.ClientIdentifier, It.IsAny<string>(), 0))
                .Callback((string s, string s2, int s3) =>
                {
                    DataPacket? packet = JsonSerializer.Deserialize<DataPacket>(s);
                    if (packet?.Header == ClientDataHeader.Image.ToString())
                    {
                        isImagePacketSent = true;
                    }
                });

            DataPacket packet = new(0, "name", ServerDataHeader.Send.ToString(), "10");
            string serializedData = JsonSerializer.Serialize(packet);

            screenshareClient.OnDataReceived(serializedData);

            Thread.Sleep(1000);

            Assert.IsTrue(isImagePacketSent);

            packet = new(0, "name", ServerDataHeader.Stop.ToString(), "10");
            serializedData = JsonSerializer.Serialize(packet);

            screenshareClient.OnDataReceived(serializedData);

            Thread.Sleep(1000);

            bool? _imageCancellationToken = (bool?)typeof(ScreenshareClient)
                            .GetField("_imageCancellationToken", BindingFlags.NonPublic | BindingFlags.Instance)!
                            .GetValue(screenshareClient);

            Assert.IsTrue(_imageCancellationToken);
            screenshareClient.StopScreensharing();
        }


    }
}

// start -> stop -> start
// moq start processing sends packets to communicator (register + communication)
// using moq test on data receive both things
// using moq test start image sending
// test stop ss
// test StopConfirmationSending
// 

*/
