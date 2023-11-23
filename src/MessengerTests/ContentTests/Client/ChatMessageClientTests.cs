/******************************************************************************
 * Filename    = ContentTests.cs
 *
 * Author      = Rapeti Siddhu Neehal
 *
 * Product     = Messenger
 * 
 * Project     = MessengerTests
 *
 * Description = chat message client tests
 *****************************************************************************/

using MessengerContent.Enums;
using MessengerContent;
using MessengerContent.Client;
using MessengerContent.DataModels;
using MessengerTests.ContentTests;

namespace MessengerTests.ContentTests.Client
{
    [TestClass]
    public class ChatClientTests
    {
        [TestMethod]
        public void ConvertSendChatData_ValidInput_ReturnsValidChatData()
        {
            var helper = new MockHelper();
            SendChatData sendChatData = helper.GenerateSendChatData(MessageType.Chat, "This is a message string");
            var chatClient = new ChatMessageClient(helper.GetMockCommunicator());

            ChatData chatData = chatClient.ChatDataFromSendData(sendChatData, MessageEvent.New);

            Assert.AreEqual(sendChatData.Type, chatData.Type);
            Assert.AreEqual(sendChatData.Data, chatData.Data);
            Assert.AreEqual(sendChatData.ReplyThreadID, chatData.ReplyThreadID);
            Assert.AreEqual(MessageEvent.New, chatData.Event);
            Assert.IsFalse(chatData.Starred);
            Assert.IsNull(chatData.FileData);
        }

        [TestMethod]
        public void NewChat_ValidInput_ReturnsValidChatData()
        {
            var helper = new MockHelper();
            SendChatData sendChatData = helper.GenerateSendChatData(MessageType.Chat, "This is a message string");
            MockCommunicator mockCommunicator = helper.GetMockCommunicator();
            var serializer = new ContentSerializer();
            int userID = 5;
            var chatClient = new ChatMessageClient(mockCommunicator)
            {
                UserID = userID,
                Communicator = mockCommunicator
            };
            ChatData chatData = chatClient.ChatDataFromSendData(sendChatData, MessageEvent.New);

            chatClient.NewChat(sendChatData);
            string serializedData = mockCommunicator.GetSendData();
            ChatData deserializedData = serializer.Deserialize<ChatData>(serializedData);

            //Assert.isType<ChatData>(deserializedData);
            Assert.AreEqual(chatData.Type, deserializedData.Type);
            Assert.AreEqual(chatData.Data, deserializedData.Data);
            Assert.AreEqual(chatData.ReplyThreadID, deserializedData.ReplyThreadID);
            Assert.AreEqual(chatData.FileData, deserializedData.FileData);
            Assert.AreEqual(chatData.SenderID, deserializedData.SenderID);
            Assert.AreEqual(chatData.Starred, deserializedData.Starred);
            Assert.AreEqual(chatData.Event, deserializedData.Event);
        }
        [TestMethod]
        public void NewChat_EmptyMessageString_ReturnsArgumentException()
        {
            var helper = new MockHelper();
            SendChatData sendChatData = helper.GenerateSendChatData(MessageType.Chat, "");
            MockCommunicator mockCommunicator = helper.GetMockCommunicator();
            var serializer = new ContentSerializer();
            int userID = 5;
            string ip = "127.0.0.1";
            int port = 55000;
            var chatClient = new ChatMessageClient(mockCommunicator)
            {
                UserID = userID,
                Communicator = mockCommunicator
            };

            Assert.ThrowsException<ArgumentException>(() => chatClient.NewChat(sendChatData));
        }
        [TestMethod]
        public void NewChat_NullMessageString_ReturnsArgumentException()
        {
            var helper = new MockHelper();
            SendChatData sendChatData = helper.GenerateSendChatData(MessageType.Chat, null);
            MockCommunicator mockCommunicator = helper.GetMockCommunicator();
            var serializer = new ContentSerializer();
            int userID = 5;
            string ip = "127.0.0.1";
            int port = 55000;
            var chatClient = new ChatMessageClient(mockCommunicator)
            {
                UserID = userID,
                Communicator = mockCommunicator
            };

            Assert.ThrowsException<ArgumentException>(() => chatClient.NewChat(sendChatData));
        }
        [TestMethod]
        public void EditChat_ValidInput_ReturnsValidContentData()
        {
            var helper = new MockHelper();
            SendChatData sendChatData = helper.GenerateSendChatData(MessageType.Chat, "This is an edited message");
            MockCommunicator mockCommunicator = helper.GetMockCommunicator();
            var serializer = new ContentSerializer();
            int userID = 5;
            int messageID = 6;
            int threadID = 7;
            var chatClient = new ChatMessageClient(mockCommunicator)
            {
                UserID = userID,
                Communicator = mockCommunicator
            };
            ChatData chatData = chatClient.ChatDataFromSendData(sendChatData, MessageEvent.Edit);

            chatClient.EditChat(messageID, "This is an edited message", threadID);
            string serializedData = mockCommunicator.GetSendData();
            ChatData deserializedData = serializer.Deserialize<ChatData>(serializedData);

            Assert.AreEqual(chatData.Type, deserializedData.Type);
            Assert.AreEqual(chatData.Data, deserializedData.Data);
            Assert.AreEqual(messageID, deserializedData.MessageID);
            Assert.AreEqual(threadID, deserializedData.ReplyThreadID);
            Assert.AreEqual(chatData.FileData, deserializedData.FileData);
            Assert.AreEqual(chatData.SenderID, deserializedData.SenderID);
            Assert.AreEqual(chatData.Starred, deserializedData.Starred);
            Assert.AreEqual(chatData.Event, deserializedData.Event);
        }

        [TestMethod]
        public void DeleteChat_ValidInput_ReturnsValidContentData()
        {
            var helper = new MockHelper();
            SendChatData sendChatData = helper.GenerateSendChatData(MessageType.Chat, "Message Deleted.");
            MockCommunicator mockCommunicator = helper.GetMockCommunicator();
            var serializer = new ContentSerializer();
            int userID = 5;
            int messageID = 6;
            int threadID = 7;
            var chatClient = new ChatMessageClient(mockCommunicator)
            {
                UserID = userID,
                Communicator = mockCommunicator
            };
            ChatData chatData = chatClient.ChatDataFromSendData(sendChatData, MessageEvent.Delete);

            chatClient.DeleteChat(messageID, threadID);
            string serializedData = mockCommunicator.GetSendData();
            ChatData deserializedData = serializer.Deserialize<ChatData>(serializedData);

            Assert.AreEqual(chatData.Type, deserializedData.Type);
            Assert.AreEqual(chatData.Data, deserializedData.Data);
            Assert.AreEqual(messageID, deserializedData.MessageID);
            Assert.AreEqual(threadID, deserializedData.ReplyThreadID);
            Assert.AreEqual(chatData.FileData, deserializedData.FileData);
            Assert.AreEqual(chatData.SenderID, deserializedData.SenderID);
            Assert.AreEqual(chatData.Starred, deserializedData.Starred);
            Assert.AreEqual(chatData.Event, deserializedData.Event);
        }

        [TestMethod]
        public void StarChat_ValidInput_ReturnsValidContentData()
        {
            var helper = new MockHelper();
            SendChatData sendChatData = helper.GenerateSendChatData(MessageType.Chat, null);
            MockCommunicator mockCommunicator = helper.GetMockCommunicator();
            var serializer = new ContentSerializer();
            int userID = 5;
            int messageID = 6;
            int threadID = 7;
            var chatClient = new ChatMessageClient(mockCommunicator)
            {
                UserID = userID,
                Communicator = mockCommunicator
            };
            ChatData chatData = chatClient.ChatDataFromSendData(sendChatData, MessageEvent.Star);

            chatClient.StarChat(messageID, threadID);
            string serializedData = mockCommunicator.GetSendData();
            ChatData deserializedData = serializer.Deserialize<ChatData>(serializedData);

            Assert.AreEqual(chatData.Type, deserializedData.Type);
            Assert.AreEqual(chatData.Data, deserializedData.Data);
            Assert.AreEqual(messageID, deserializedData.MessageID);
            Assert.AreEqual(threadID, deserializedData.ReplyThreadID);
            Assert.AreEqual(chatData.FileData, deserializedData.FileData);
            Assert.AreEqual(chatData.SenderID, deserializedData.SenderID);
            Assert.AreEqual(chatData.Event, deserializedData.Event);
        }
    }
}
