/******************************************************************************
* Filename    = ChatServerTests.cs
*
* Author      = Likhitha
*
* Product     = Messenger
* 
* Project     = MessengerContent
*
* Description = Contains Tests for ChatServer
*****************************************************************************/

using MessengerContent;
using MessengerContent.DataModels;
using MessengerContent.Enums;
using MessengerContent.Server;

namespace MessengerContentTests.ContentTests
{
    public class ChatServerTests
    {
        public MockHelper _utils;
        public ContentDataBase database;
        public ChatServer ChatServer;

        public void Setup()
        {
            database = new ContentDataBase();
            ChatServer = new ChatServer(database);
            _utils = new MockHelper();
        }

        [TestMethod]
        public void GetMessages_ValidInput_ReturnsAllMessages()
        {
            Setup();
            ChatData msg = _utils.GenerateChatData(MessageType.Chat, data: "Testing Get Messages");
            ChatData msg1 = ChatServer.Receive(msg);
            List<ChatThread> allMessage = ChatServer.GetMessages();
            Assert.NotNull(allMessage);
        }

        [TestMethod]
        public void UpdateChat_ValidInput_ReturnsValidContentData()
        {
            Setup();
            ChatData msg = _utils.GenerateChatData(MessageType.Chat, data: "Testing Update");
            ChatData msg1 = ChatServer.Receive(msg);
            ReceiveChatData updatedMsg = ChatServer.UpdateMessage(msg1.ReplyThreadID, msg1.MessageID, "Message Updated");
            Assert.Equal("Message Updated", updatedMsg.Data);
        }

        [TestMethod]
        public void StarMessage_ValidInput_ReturnsTrue()
        {
            Setup();
            ChatData msg = _utils.GenerateChatData(MessageType.Chat, data: "Testing Starring");
            ChatData msg1 = ChatServer.Receive(msg);
            ReceiveChatData starredMsg = ChatServer.StarMessage(msg1.ReplyThreadID, msg1.MessageID);
            Assert.True(starredMsg.Starred);
        }

        [TestMethod]
        public void DeleteChat_ValidInput_ReturnsValidContentData()
        {
            Setup();
            ChatData msg = _utils.GenerateChatData(MessageType.Chat, data: "Testing Delete");
            ChatData msg1 = ChatServer.Receive(msg);
            ReceiveChatData updatedMsg = ChatServer.DeleteMessage(msg1.ReplyThreadID, msg1.MessageID);
            Assert.Equal("Message Deleted.", updatedMsg.Data);
        }

        [TestMethod]
        public void Receive_UpdatingMessage_ReturnsUpdatedMessage()
        {
            Setup();
            var msg1 = _utils.GenerateChattData(data: "Test Message", senderID: 1);
            ReceiveChatData receivedMsg = ChatServer.Receive(msg1);
            Assert.Equal(msg1.Data, receivedMsg.Data);

            var updateMessage = new ChatData
            {
                Data = "Testing Update",
                MessageID = receivedMsg.MessageID,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Edit
            };

            receivedMsg = ChatServer.Receive(updateMessage);

            Assert.NotNull(receivedMsg);
            Assert.Equal("Testing Update", receivedMsg.Data);
        }

        [TestMethod]
        public void Receive_UpdatingMessageDoesntExist_ReturnsNull()
        {
            Setup();
            var msg1 = _utils.GenerateChatData(data: "Test Message", senderID: 1);

            ReceiveChatData receivedMsg = ChatServer.Receive(msg1);

            Assert.Equal(msg1.Data, receivedMsg.Data);
            Assert.Equal(msg1.Type, receivedMsg.Type);

            var updateMessage = new ChatData
            {
                Data = "Testing Update",
                MessageID = 1,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Edit
            };

            receivedMsg = ChatServer.Receive(updateMessage);
            updateMessage.MessageID = 0;
            updateMessage.ReplyThreadID = 1;
            receivedMsg = ChatServer.Receive(updateMessage);
            Assert.Null(receivedMsg);
        }

        [TestMethod]
        public void Receive_StoringMultipleMessages_AllMessagesReturned()
        {
            database = new ContentDataBase();
            ChatServer = new ChatServer(database);
            _utils = new MockHelper();

            var msg1 = _utils.GenerateChatData(data: "Test Message", senderID: 1);
            ReceiveChatData receivedMsg = ChatServer.Receive(msg1);
            Assert.Equal(msg1.Data, receivedMsg.Data);

            var msg2 = _utils.GenerateChatData(data: "Test Message2", senderID: 1, replyThreadID: msg1.ReplyThreadID);
            receivedMsg = ChatServer.Receive(msg2);
            Assert.Equal(msg2.Data, receivedMsg.Data);
            Assert.NotEqual(msg2.MessageID, msg1.MessageID);

            var msg3 = _utils.GenerateChatData(data: "Test Message3", senderID: 1);
            receivedMsg = ChatServer.Receive(msg3);
            Assert.Equal(msg3.Data, receivedMsg.Data);
            Assert.NotEqual(msg3.MessageID, msg2.MessageID);
        }

        [TestMethod]
        public void Receive_NewMessage_StoreMessageAn_ReturnStoredMessage()
        {
            Setup();
            var msg1 = _utils.GenerateChatData(data: "Test Message", senderID: 1);
            ReceiveChatData receivedMsg = ChatServer.Receive(msg1);
            Assert.Equal(msg1.Data, receivedMsg.Data);
        }

        [TestMethod]
        public void Receive_StarringMessage_ReturnsTheStarredMessage()
        {
            Setup();
            var msg = _utils.GenerateChatData(data: "Test Message", senderID: 1);
            ReceiveChatData receivedMsg = ChatServer.Receive(msg);
            Assert.Equal(msg.Data, receivedMsg.Data);

            var starMessage = new ChatData
            {
                MessageID = receivedMsg.MessageID,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = receivedMsg.Type,
                Event = MessageEvent.Star
            };

            receivedMsg = ChatServer.Receive(starMessage);
            Assert.NotNull(receivedMsg);
            Assert.Equal("Test Message", receivedMsg.Data);
        }

        [Fact]
        public void Receive_StarringMessageDoesNotExist_ReturnsNull()
        {
            Setup();
            var msg = _utils.GenerateChatData(data: "Test Message", senderID: 1);
            ReceiveChatData receivedMsg = ChatServer.Receive(msg);
            Assert.Equal(msg.Data, receivedMsg.Data);

            var starMessage = new ChatData
            {
                MessageID = 1,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Star
            };

            receivedMsg = ChatServer.Receive(starMessage);
            starMessage.MessageID = 0;
            starMessage.ReplyThreadID = 1;
            receivedMsg = ChatServer.Receive(starMessage);
            Assert.Null(receivedMsg);
        }

        [Fact]
        public void Receive_DeletingtingMessage_ReturnsMessageDeleted()
        {
            Setup();
            var msg1 = _utils.GenerateChatData(data: "Test Message", senderID: 1);
            ReceiveChatData receivedMsg = ChatServer.Receive(msg1);
            Assert.Equal(msg1.Data, receivedMsg.Data);

            var deleteMessage = new ChatData
            {
                MessageID = receivedMsg.MessageID,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Delete
            };

            receivedMsg = ChatServer.Receive(deleteMessage);
            Assert.NotNull(receivedMsg);
            Assert.Equal("Message Deleted.", receivedMsg.Data);
        }

        [Fact]
        public void Receive_DeletingMessageDoesNotExist_ReturnsNull()
        {
            Setup();
            var msg = _utils.GenerateChatData(data: "Test Message", senderID: 1);
            ReceiveChatData receivedMsg = ChatServer.Receive(msg);
            Assert.Equal(msg.Data, receivedMsg.Data);

            var delMessage = new ChatData
            {
                MessageID = 1,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Delete
            };

            receivedMsg = ChatServer.Receive(delMessage);
            delMessage.MessageID = 0;
            delMessage.ReplyThreadID = 1;
            receivedMsg = ChatServer.Receive(delMessage);
            Assert.Null(receivedMsg);
        }

    }
}
