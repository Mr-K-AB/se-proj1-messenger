using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerNetworking.Communicator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessengerTests.MessengerNetworking.Communicator
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            string serverIp = "127.0.0.1";
            int serverPort = 12345;
            string messageToSend = "Hello, client!";

            ServerCommunicator server = new( serverPort );
            server.Start();

            ClientCommunicator client = new( serverIp , serverPort );
            client.Connect();


            /// review what senderModule requires and serverIp too
            client.Send( messageToSend , serverIp, serverIp );

            Assert.AreEqual( messageToSend , server.latestMessage );

            // client.Disconnect();
            // server.Stop();
        }
    }
}
