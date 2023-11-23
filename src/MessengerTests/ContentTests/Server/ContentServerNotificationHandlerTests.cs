/******************************************************************************
 * Filename    = ChatServerNotificationHandler.cs
 *
 * Author      = Manikanta Gudipudi
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContentTests
 *
 * Description = Tests for ChatServerNotificationHandler
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerContent.DataModels;
using MessengerContent.Enums;
using MessengerContent.Server;
using MessengerContent;
using MessengerNetworking.NotificationHandler;

namespace MessengerTests.ContentTests.Server
{
    [TestClass]
    public class ContentServerNotificationHandlerTests
    {
        private MockCommunicator _communicator;
        private ContentServer _contentServer;
        private MockMessageListener _listener;
        private INotificationHandler _notifHandler;
        private IContentSerializer _serializer;
        private int _sleeptime;
        private MockHelper _helper;

        [TestMethod]
        public void OnDataReceived_ChatIsReceived_CallReceiveMethodOfContentDB()
        {
            _helper = new MockHelper();
            _contentServer = ContentServerFactory.GetInstance() as ContentServer;
            _contentServer.Reset();
            _sleeptime = 50;
            _notifHandler = new ContentServerNotificationHandler(_contentServer);
            _serializer = new ContentSerializer();
            _listener = new MockMessageListener();
            _communicator = new MockCommunicator();
            _contentServer.Communicator = _communicator;
            _contentServer.ServerSubscribe(_listener);

            string CurrentDirectory = Directory.GetCurrentDirectory();
            string[] path = CurrentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            string pathA = path[0] + "\\MessengerTests\\ContentTests\\TestFile.txt";

            var file = new ChatData
            {
                Data = "TestFile.txt",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New,
            };

            string serializedMessage = _serializer.Serialize(file);

            _notifHandler.OnDataReceived(serializedMessage);

            Thread.Sleep(_sleeptime);

            ReceiveChatData notifiedMessage = _listener.GetReceivedMessage();

            Assert.AreEqual("TestFile.txt", notifiedMessage.Data);
        }
    }
}
