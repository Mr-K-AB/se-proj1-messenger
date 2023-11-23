/******************************************************************************
 * Filename    = ChatServerTests.cs
 *
 * Author      = Manikanta Gudipudi
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContentTests
 *
 * Description = Tests for ChatServer
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerContent;
using MessengerContent.DataModels;
using MessengerContent.Enums;
using MessengerContent.Server;

namespace MessengerTests.ContentTests.Server
{
    [TestClass]
    public class ChatServerTests
    {
        public MockHelper _helper;
        public ContentDataBase database;
        public ChatServer? ChatServer;

        public void Setup()
        {
            database = new ContentDataBase();
            ChatServer = new ChatServer(database);
            _helper = new MockHelper();
        }

        [TestMethod]
        public void GetMessages_ValidInput_ReturnsAllMessages()
        {
            Setup();
            ChatData msg = _helper.GenerateChatData(MessageType.Chat, data: "Testing Get Messages");
            _ = ChatServer.Receive(msg);
            List<ChatThread> allMessage = ChatServer.GetMessages();
            Assert.IsNotNull(allMessage);
        }


        [TestMethod]
        public void UpdateChat_ValidInput_ReturnsValidContentData()
        {
            Setup();
            ChatData msg = _helper.GenerateChatData(MessageType.Chat, data: "Testing Update");
            ChatData msg1 = ChatServer.Receive(msg);
            ReceiveChatData updatedMsg = ChatServer.UpdateMessage(msg1.ReplyThreadID, msg1.MessageID, "Message Updated");
            Assert.AreEqual("Message Updated", updatedMsg.Data);
        }

        [TestMethod]
        public void StarMessage_ValidInput_ReturnsTrue()
        {
            Setup();
            ChatData msg = _helper.GenerateChatData(MessageType.Chat, data: "Testing Starring");
            ChatData msg1 = ChatServer.Receive(msg);
            ReceiveChatData starredMsg = ChatServer.StarMessage(msg1.ReplyThreadID, msg1.MessageID);
            Assert.IsTrue(starredMsg.Starred);
        }

        [TestMethod]
        public void DeleteChat_ValidInput_ReturnsValidContentData()
        {
            Setup();
            ChatData msg = _helper.GenerateChatData(MessageType.Chat, data: "Testing Delete");
            ChatData msg1 = ChatServer.Receive(msg);
            ReceiveChatData updatedMsg = ChatServer.DeleteMessage(msg1.ReplyThreadID, msg1.MessageID);
            Assert.AreEqual("Message Deleted.", updatedMsg.Data);
        }

        [TestMethod]
        public void Receive_UpdatingMessage_ReturnsUpdatedMessage()
        {
            Setup();
            ChatData msg1 = _helper.GenerateChatData(data: "Test Message", senderID: 1);
            ReceiveChatData receivedMsg = ChatServer.Receive(msg1);
            Assert.AreEqual(msg1.Data, receivedMsg.Data);

            var updateMessage = new ChatData
            {
                Data = "Testing Update",
                MessageID = receivedMsg.MessageID,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Edit
            };

            receivedMsg = ChatServer.Receive(updateMessage);

            Assert.IsNotNull(receivedMsg);
            Assert.AreEqual("Testing Update", receivedMsg.Data);
        }

        [TestMethod]
        public void Receive_InvalidMessageEvent_ReturnsNull()
        {
            Setup();
            ChatData msg1 = _helper.GenerateChatData(data: "Test Message", senderID: 1);
            msg1.Event += 6;
            ReceiveChatData receivedMsg = ChatServer.Receive(msg1);
            Assert.IsNull(receivedMsg);
        }

        [TestMethod]
        public void Receive_UpdatingMessageDoesntExist_ReturnsNull()
        {
            Setup();
            ChatData msg1 = _helper.GenerateChatData(data: "Test Message", senderID: 1);

            ChatData receivedMsg = ChatServer.Receive(msg1);

            Assert.AreEqual(msg1.Data, receivedMsg.Data);
            Assert.AreEqual(msg1.Type, receivedMsg.Type);

            var updateMessage = new ChatData
            {
                Data = "Testing Update",
                MessageID = 1,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Edit
            };

            _ = ChatServer.Receive(updateMessage);
            updateMessage.MessageID = 0;
            updateMessage.ReplyThreadID = 1;
            receivedMsg = ChatServer.Receive(updateMessage);
            Assert.IsNull(receivedMsg);
        }

        [TestMethod]
        public void Receive_StoringMultipleMessages_AllMessagesReturned()
        {
            database = new ContentDataBase();
            ChatServer = new ChatServer(database);
            _helper = new MockHelper();

            ChatData msg1 = _helper.GenerateChatData(data: "Test Message", senderID: 1);
            ChatData receivedMsg = ChatServer.Receive(msg1);
            Assert.AreEqual(msg1.Data, receivedMsg.Data);

            ChatData msg2 = _helper.GenerateChatData(data: "Test Message2", senderID: 1, replyThreadID: msg1.ReplyThreadID);
            receivedMsg = ChatServer.Receive(msg2);
            Assert.AreEqual(msg2.Data, receivedMsg.Data);
            Assert.AreNotEqual(msg2.MessageID, msg1.MessageID);

            ChatData msg3 = _helper.GenerateChatData(data: "Test Message3", senderID: 1);
            receivedMsg = ChatServer.Receive(msg3);
            Assert.AreEqual(msg3.Data, receivedMsg.Data);
            Assert.AreNotEqual(msg3.MessageID, msg2.MessageID);
        }

        [TestMethod]
        public void Receive_NewMessage_StoreMessageAn_ReturnStoredMessage()
        {
            Setup();
            ChatData msg1 = _helper.GenerateChatData(data: "Test Message", senderID: 1);
            ReceiveChatData receivedMsg = ChatServer.Receive(msg1);
            Assert.AreEqual(msg1.Data, receivedMsg.Data);
        }

        [TestMethod]
        public void Receive_StarringMessage_ReturnsTheStarredMessage()
        {
            Setup();
            ChatData msg = _helper.GenerateChatData(data: "Test Message", senderID: 1);
            ReceiveChatData receivedMsg = ChatServer.Receive(msg);
            Assert.AreEqual(msg.Data, receivedMsg.Data);

            var starMessage = new ChatData
            {
                MessageID = receivedMsg.MessageID,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = receivedMsg.Type,
                Event = MessageEvent.Star
            };

            receivedMsg = ChatServer.Receive(starMessage);
            Assert.IsNotNull(receivedMsg);
            Assert.AreEqual("Test Message", receivedMsg.Data);
        }

        [TestMethod]
        public void Receive_StarringMessageDoesNotExist_ReturnsNull()
        {
            Setup();
            ChatData msg = _helper.GenerateChatData(data: "Test Message", senderID: 1);
            ReceiveChatData receivedMsg = ChatServer.Receive(msg);
            Assert.AreEqual(msg.Data, receivedMsg.Data);

            var starMessage = new ChatData
            {
                MessageID = 1,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Star
            };

            _ = ChatServer.Receive(starMessage);
            starMessage.MessageID = 0;
            starMessage.ReplyThreadID = 1;
            receivedMsg = ChatServer.Receive(starMessage);
            Assert.IsNull(receivedMsg);
        }

        [TestMethod]
        public void Receive_DeletingtingMessage_ReturnsMessageDeleted()
        {
            Setup();
            ChatData msg1 = _helper.GenerateChatData(data: "Test Message", senderID: 1);
            ReceiveChatData receivedMsg = ChatServer.Receive(msg1);
            Assert.AreEqual(msg1.Data, receivedMsg.Data);

            var deleteMessage = new ChatData
            {
                MessageID = receivedMsg.MessageID,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Delete
            };

            receivedMsg = ChatServer.Receive(deleteMessage);
            Assert.IsNotNull(receivedMsg);
            Assert.AreEqual("Message Deleted.", receivedMsg.Data);
        }

        [TestMethod]
        public void Receive_DeletingMessageDoesNotExist_ReturnsNull()
        {
            Setup();
            ChatData msg = _helper.GenerateChatData(data: "Test Message", senderID: 1);
            ReceiveChatData receivedMsg = ChatServer.Receive(msg);
            Assert.AreEqual(msg.Data, receivedMsg.Data);

            var delMessage = new ChatData
            {
                MessageID = 1,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Delete
            };

            _ = ChatServer.Receive(delMessage);
            delMessage.MessageID = 0;
            delMessage.ReplyThreadID = 1;
            receivedMsg = ChatServer.Receive(delMessage);
            Assert.IsNull(receivedMsg);
        }

    }
}
