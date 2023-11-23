/******************************************************************************
 * 
 * Author      = A Sathvik
 *
 * Roll no     = 112001005
 *
 *****************************************************************************/
using MessengerNetworking.Communicator;
using MessengerScreenshare.Client;
using MessengerScreenshare;
using Moq;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using SSUtils = MessengerScreenshare.Utils;
using System.Security.Cryptography;

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

            string argString = "";
            communicatorMock.Setup(p => p.Send(It.IsAny<string>(), SSUtils.ServerIdentifier, null))
                .Callback((string s, string s2, string s3) => { if (argString == "") { argString = s; } });

            screenshareClient.SetUser(1, "name");
            Task.Run(async() => await screenshareClient.StartScreensharingAsync());

            while (argString == "") {}

            DataPacket? packet = JsonSerializer.Deserialize<DataPacket>(argString);

            Assert.IsTrue(packet?.Header == ClientDataHeader.Register.ToString());
        }

        [TestMethod]
        public void TestSendPacketReceive()
        {
            ScreenshareClient screenshareClient = ScreenshareClient.GetInstance(isDebugging: true);

            var communicatorMock = new Mock<ICommunicator>();
            screenshareClient.SetPrivate("_communicator", communicatorMock.Object);

            screenshareClient.SetUser(2, "clientName");
            Task.Run(async () => await screenshareClient.StartScreensharingAsync());

            bool isImagePacketSent = false;
            communicatorMock.Setup(p => p.Send(It.IsAny<string>(), SSUtils.ClientIdentifier, null))
                .Callback((string s, string s2, string s3) =>
                {
                    DataPacket? packet = JsonSerializer.Deserialize<DataPacket>(s);
                    if (packet?.Header == ClientDataHeader.Image.ToString())
                    {
                        isImagePacketSent = true;
                    }
                });

            DataPacket packet = new(1, "serverName", ServerDataHeader.Send.ToString(), "2");
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

            screenshareClient.SetUser(2, "clientName");
            Task.Run(async () => await screenshareClient.StartScreensharingAsync());

            bool isImagePacketSent = false;
            communicatorMock.Setup(p => p.Send(It.IsAny<string>(), SSUtils.ClientIdentifier, null))
                .Callback((string s, string s2, string s3) =>
                {
                    DataPacket? packet = JsonSerializer.Deserialize<DataPacket>(s);
                    if (packet?.Header == ClientDataHeader.Image.ToString())
                    {
                        isImagePacketSent = true;
                    }
                });

            DataPacket packet = new(1, "serverName", ServerDataHeader.Send.ToString(), "2");
            string serializedData = JsonSerializer.Serialize(packet);

            screenshareClient.OnDataReceived(serializedData);

            Thread.Sleep(1000);

            packet = new(1, "serverName", ServerDataHeader.Stop.ToString(), "2");
            serializedData = JsonSerializer.Serialize(packet);

            screenshareClient.OnDataReceived(serializedData);

            Thread.Sleep(1000);

            CancellationTokenSource? _imageCancellationToken = (CancellationTokenSource?)typeof(ScreenshareClient)
                            .GetField("_imageCancellation", BindingFlags.NonPublic | BindingFlags.Instance)!
                            .GetValue(screenshareClient);

            Assert.IsTrue(_imageCancellationToken.IsCancellationRequested);
            screenshareClient.StopScreensharing();
        }


    }
}
