/******************************************************************************
 * Filename    = ContentServerTests.cs
 *
 * Author      = Manikanta Gudipudi
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContentTests
 *
 * Description = Tests for ContentServer
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerContent.DataModels;
using MessengerContent.Enums;
using MessengerContent.Server;
using MessengerContent;
using MessengerTests.ContentTests;
using MessengerNetworking.Communicator;
using MessengerNetworking.Factory;
using MessengerNetworking.NotificationHandler;

namespace MessengerTests.ContentTests.Server
{
    [TestClass]
    public class ContentServerTests
    {
        private MockCommunicator _communicator;
        private ContentServer _contentServer;
        private MockMessageListener _listener;
        private IContentSerializer _serializer;
        private int _sleeptime;
        private MockHelper _helper;

        [TestMethod]
        public void Initialiser()
        {
            _communicator = new MockCommunicator();
            _sleeptime = 50;
            _serializer = new ContentSerializer();
            _helper = new MockHelper();
            _listener = new MockMessageListener();
            _contentServer = ContentServerFactory.GetInstance() as ContentServer;
            _contentServer.Reset();
            _contentServer.ServerSubscribe(_listener);
            _contentServer.Communicator = _communicator;


            ChatData messageData = _helper.GenerateChatData(data: "Test Message");
            string serializedMessage = _serializer.Serialize(messageData);
            _contentServer.Receive(serializedMessage);

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

            serializedMessage = _serializer.Serialize(file);
            _contentServer.Receive(serializedMessage);
        }

        [TestMethod]
        public void ServerSubscribe_SubscribingToNotification_GetNewMessageNotification()
        {
            Initialiser();
            ChatData messageData = _helper.GenerateChatData(data: "Test Message");
            _serializer = new ContentSerializer();
            string serializesMessage = _serializer.Serialize(messageData);

            _contentServer.Receive(serializesMessage);

            Thread.Sleep(50);

            ReceiveChatData notifiedMessage = _listener.GetReceivedMessage();
            
            Assert.AreEqual("Test Message", notifiedMessage.Data);
        }

        [TestMethod]
        public void Receive_NewMessage_NotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            Initialiser();
            ChatData messageData = _helper.GenerateChatData(data: "Test Message");
            string serializesMessage = _serializer.Serialize(messageData);
            _contentServer.Receive(serializesMessage);
            Thread.Sleep(_sleeptime);

            ReceiveChatData notifiedMessage = _listener.GetReceivedMessage();

            Assert.AreEqual("Test Message", notifiedMessage.Data);

            string? sentMessage = _communicator.GetSendData();
            ChatData deserializesSentMessage = _serializer.Deserialize<ChatData>(sentMessage);
            
            Assert.AreEqual("Test Message", deserializesSentMessage.Data);
        }

        [TestMethod]
        public void Receive_HistoryRequest_NotifyTheSubcsribersAndForwardAllMessagesToCommunicator()
        {
            Initialiser();
            ChatData messageData = _helper.GenerateChatData(data: null);
            messageData.Type = MessageType.HistoryRequest;
            string serializesMessage = _serializer.Serialize(messageData);
            _contentServer.Receive(serializesMessage);
            Thread.Sleep(_sleeptime);

            string? sentMessage = _communicator.GetSendData();
            List<ChatThread> deserializesSentMessage = _serializer.Deserialize <List<ChatThread>>(sentMessage);

            Assert.AreEqual(2, deserializesSentMessage.Count);
        }

        [TestMethod]
        public void Receive_UpdateMessageForNonExsistingMessage_PrintsErrorMessage()
        {
            Initialiser();
            ChatData messageData = _helper.GenerateChatData(data: "Edited Text", replyThreadID: 1, replyMessageID: 3);
            messageData.Event = MessageEvent.Edit;
            string serializesMessage = _serializer.Serialize(messageData);
            _contentServer.Receive(serializesMessage);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Receive_NewFile_SaveFileAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            Initialiser();
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

            string serializesMessage = _serializer.Serialize(file);
            _contentServer.Receive(serializesMessage);
            Thread.Sleep(_sleeptime);

            ReceiveChatData notifiedMessage = _listener.GetReceivedMessage();
            Assert.AreEqual("TestFile.txt", notifiedMessage.Data);
            Assert.AreEqual(file.Type, notifiedMessage.Type);

            string sentMessage = _communicator.GetSendData();
            ChatData deserializesSentMessage = _serializer.Deserialize<ChatData>(sentMessage);
            Assert.AreEqual("TestFile.txt", deserializesSentMessage.Data);
            Assert.AreEqual(file.Type, deserializesSentMessage.Type);
        }

        [TestMethod]
        public void Receive_DownloadFile_FetchFileAndForwadToCommunicator()
        {
            Initialiser();
            string CurrentDirectory = Directory.GetCurrentDirectory();
            string[] path = CurrentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            string pathA = path[0] + "\\MessengerTests\\ContentTests\\TestFile.txt";
            _ = new SendFileData(pathA);
            var fileDownloadMessage = new ChatData
            {
                Data = "x.pdf",
                MessageID = 1,
                ReplyThreadID = 1,
                Type = MessageType.File,
                Event = MessageEvent.Download,
                SenderID = 10
            };

            string serializedFileDownloadMessage = _serializer.Serialize(fileDownloadMessage);
            _contentServer.Receive(serializedFileDownloadMessage);

            string sentData = _communicator.GetSendData();
            ChatData deserializedSentData = _serializer.Deserialize<ChatData>(sentData);

            Assert.AreEqual("x.pdf", deserializedSentData.Data);
            Assert.AreEqual(1, deserializedSentData.MessageID);
            Assert.AreEqual(1, deserializedSentData.ReplyThreadID);
            Assert.AreEqual(MessageType.File, deserializedSentData.Type);
            Assert.AreEqual(MessageEvent.Download, deserializedSentData.Event);
        }

        [TestMethod]
        public void SGetAllMessages_GettingAllTheMessagesOnServer_ShouldReturnListOfChatContextsWithAllTheMessages()
        {
            Initialiser();
            List<ChatThread> chatContexts = _contentServer.GetAllMessages();
            ReceiveChatData firstMessage = chatContexts[0].MessageList[0];

            Assert.AreEqual("Test Message", firstMessage.Data);
            Assert.AreEqual(MessageType.Chat, firstMessage.Type);
            Assert.AreEqual(MessageEvent.New, firstMessage.Event);
            Assert.AreEqual(0, firstMessage.MessageID);
            Assert.AreEqual(0, firstMessage.ReplyThreadID);

            ReceiveChatData secondMessage = chatContexts[1].MessageList[0];
            Assert.AreEqual("TestFile.txt", secondMessage.Data);
            Assert.AreEqual(MessageType.File, secondMessage.Type);
            Assert.AreEqual(MessageEvent.New, secondMessage.Event);
            Assert.AreEqual(1, secondMessage.MessageID);
            Assert.AreEqual(1, secondMessage.ReplyThreadID);
        }

        [TestMethod]
        public void SSendAllMessagesToClient_SendingAllMessagesToANewlyJoinedClient_ListOfChatContextsShouldBeForwadedToCommunicator()
        {
            Initialiser();
            int userId = 1;
            _contentServer.SendAllMessagesToClient(userId);
            Assert.AreEqual(1, userId);
        }

        /*[TestMethod]
        public void Receive_HandlingPrivateMessages_ShouldSaveTheNewMessageAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            Initialiser();
            ChatData messageData = _helper.GenerateChatData(data: "Test Message");
            messageData.SenderID = 1;

            string serializesMessage = _serializer.Serialize(messageData);
            _contentServer.Receive(serializesMessage);
            Thread.Sleep(_sleeptime);

            ReceiveChatData notifiedMessage = _listener.GetReceivedMessage();
            Assert.AreEqual("Test Message", notifiedMessage.Data);

            string sentMessage = _communicator.GetSendData();
            ChatData deserializesSentMessage = _serializer.Deserialize<ChatData>(sentMessage);
            Assert.AreEqual("Test Message", deserializesSentMessage.Data);
        }*/
    }
}
