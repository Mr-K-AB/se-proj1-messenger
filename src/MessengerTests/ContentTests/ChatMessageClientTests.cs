/******************************************************************************
 * Filename    = ContentTests.cs
 *
 * Author      = Rapeti Siddhu Neehal
 *
 * Product     = Messenger
 * 
 * Project     = MessengerTests
 *
 * Description = tests
 *****************************************************************************/

using MessengerContent.Enums;
using MessengerContent;
using MessengerContent.Client;
using MessengerContent.DataModels;
using MessengerTests.ContentTests;

namespace MessnegerTests.ContentTests
{
    [TestClass]
    public class ChatClientTests
    {
        [TestMethod]
        public void ConvertSendChatData_ValidInput_ReturnsValidChatData()
        {
            var utility = new MockHelper();
            SendChatData sendContentData = utility.GenerateSendChatData(MessageType.Chat, "This is a message string");
            var chatClient = new ChatMessageClient(utility.GetMockCommunicator());

            ChatData contentData = chatClient.ChatDataFromSendData(sendContentData, MessageEvent.New);

            Assert.AreEqual(sendContentData.Type, contentData.Type);
            Assert.AreEqual(sendContentData.Data, contentData.Data);
            Assert.AreEqual(sendContentData.ReplyThreadID, contentData.ReplyThreadID);
            Assert.AreEqual(MessageEvent.New, contentData.Event);
            Assert.IsFalse(contentData.Starred);
            Assert.IsNull(contentData.FileData);
        }

        [TestMethod]
        public void NewChat_ValidInput_ReturnsValidChatData()
        {
            var utility = new MockHelper();
            SendChatData sendContentData = utility.GenerateSendChatData(MessageType.Chat, "This is a message string");
            MockCommunicator mockCommunicator = utility.GetMockCommunicator();
            var serializer = new ContentSerializer();
            int userID = 5;
            var chatClient = new ChatMessageClient(mockCommunicator)
            {
                UserID = userID,
                Communicator = mockCommunicator
            };
            ChatData contentData = chatClient.ChatDataFromSendData(sendContentData, MessageEvent.New);

            chatClient.NewChat(sendContentData);
            string serializedData = mockCommunicator.GetSendData();
            ChatData deserializedData = serializer.Deserialize<ChatData>(serializedData);

            //Assert.isType<ChatData>(deserializedData);
            Assert.AreEqual(contentData.Type, deserializedData.Type);  
            Assert.AreEqual(contentData.Data, deserializedData.Data);
            Assert.AreEqual(contentData.ReplyThreadID, deserializedData.ReplyThreadID);
            Assert.AreEqual(contentData.FileData, deserializedData.FileData);
            Assert.AreEqual(contentData.SenderID, deserializedData.SenderID);
            Assert.AreEqual(contentData.Starred, deserializedData.Starred);
            Assert.AreEqual(contentData.Event, deserializedData.Event);
        }
    }
}
