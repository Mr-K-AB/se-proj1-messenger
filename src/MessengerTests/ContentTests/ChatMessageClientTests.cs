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

namespace PlexShareTests.ContentTests.Client
{
    [TestClass]
    public class ChatClientTests
    {
        [TestMethod]
        public void ConvertSendContentData_ValidInput_ReturnsValidContentData()
        {
            var utility = new MockHelper();
            SendChatData sendContentData = utility.GenerateSendChatData(MessageType.Chat, "This is a message string");
            var chatClient = new ChatMessageClient(utility.GetMockCommunicator());

            ChatData contentData = chatClient.ChatDataFromSendData(sendContentData, MessageEvent.New);

            Assert.Equals(sendContentData.Type, contentData.Type);
            Assert.Equals(sendContentData.Data, contentData.Data);
            Assert.Equals(sendContentData.ReplyThreadID, contentData.ReplyThreadID);
            Assert.Equals(MessageEvent.New, contentData.Event);
            Assert.IsFalse(contentData.Starred);
            Assert.IsNull(contentData.FileData);
        }

        [TestMethod]
        public void NewChat_ValidInput_ReturnsValidContentData()
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
            Assert.Equals(contentData.Type, deserializedData.Type);  
            Assert.Equals(contentData.Data, deserializedData.Data);
            Assert.Equals(contentData.ReplyThreadID, deserializedData.ReplyThreadID);
            Assert.Equals(contentData.FileData, deserializedData.FileData);
            Assert.Equals(contentData.SenderID, deserializedData.SenderID);
            Assert.Equals(contentData.Starred, deserializedData.Starred);
            Assert.Equals(contentData.Event, deserializedData.Event);
        }
    }
}
