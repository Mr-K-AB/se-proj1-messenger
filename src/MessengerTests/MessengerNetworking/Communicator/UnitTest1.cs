using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerNetworking.Communicator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessengerTests.MessengerNetworking.Communicator
{

    /// <summary>
    /// Checking Broadcast method of client
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        private readonly string _serverIp = "127.0.0.1";
        private readonly int _serverPort = 12345;

        [TestMethod]
        public void TestMethod1()
        {
            string messageToSend = "Hello, client!";

            ServerCommunicator server = new ( _serverPort );
            server.Start();

            ClientCommunicator client = new ( _serverIp , _serverPort );
            client.Connect();


            /// review what senderModule requires
            client.Broadcast( messageToSend , _serverIp );

            Assert.AreEqual( messageToSend , server.latestMessage );

            // client.Disconnect();
            // server.Stop();
        }

        [TestMethod]
        public void ServerListeningTest()
        {
            
        }
    }
}
