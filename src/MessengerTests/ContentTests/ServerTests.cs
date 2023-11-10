using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerContent.DataModels;
using MessengerContent.Server;
using MessengerContent;
using MessengerTests.ContentTests;

namespace MessengerTests.ContentTests
{
    [TestClass]
    public class ServerTests
    {
        public MockUtils? _utils;
        public ContentDataBase? database;
        public ChatServer? ChatServer;

        public void Setup()
        {
            database = new ContentDataBase();
            ChatServer = new ChatServer(database);
            _utils = new MockUtils();
        }

        [TestMethod]
        public void GetMessages_ValidInput_ReturnsAllMessages()
        {
            Setup();
            ChatData msg = _utils.GenerateChatData(MessageType.Chat, data: "Testing Get Messages");
            _ = ChatServer.Receive(msg);
            List<ChatThread> allMessage = ChatServer.GetMessages();
            Assert.IsNotNull(allMessage);
        }
        [TestMethod]
        public void UpdateChat_ValidInput_ReturnsValidContentData()
        {
            Setup();
            ChatData msg = _utils.GenerateChatData(MessageType.Chat, data: "Testing Update");
            ChatData msg1 = ChatServer.Receive(msg);
            ReceiveChatData updatedMsg = ChatServer.UpdateMessage(msg1.ReplyThreadID, msg1.MessageID, "Message Updated");
            Assert.AreEqual("Message Updated", updatedMsg.Data);
        }

        [TestMethod]
        public void StarMessage_ValidInput_ReturnsTrue()
        {
            Setup();
            ChatData msg = _utils.GenerateChatData(MessageType.Chat, data: "Testing Starring");
            ChatData msg1 = ChatServer.Receive(msg);
            ReceiveChatData starredMsg = ChatServer.StarMessage(msg1.ReplyThreadID, msg1.MessageID);
            Assert.IsTrue(starredMsg.Starred);
        }

        [TestMethod]
        public void DeleteChat_ValidInput_ReturnsValidContentData()
        {
            Setup();
            ChatData msg = _utils.GenerateChatData(MessageType.Chat, data: "Testing Delete");
            ChatData msg1 = ChatServer.Receive(msg);
            ReceiveChatData updatedMsg = ChatServer.DeleteMessage(msg1.ReplyThreadID, msg1.MessageID);
            Assert.AreEqual("Message Deleted.", updatedMsg.Data);
        }
    }
}
