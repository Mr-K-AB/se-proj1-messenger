/******************************************************************************
* Filename    = ContentClientNotificationHandlerTests.cs
*
* Author      = Likhitha
*
* Product     = Messenger
* 
* Project     = MessengerTests
*
* Description = Tests for content client notification handler class.    
*****************************************************************************/

using MessengerContent;
using MessengerContent.Client;
using MessengerContent.DataModels;
using MessengerContent.Enums;
using MessengerTests.ContentTests;

namespace MessengerTests.ContentTests
{
    [TestClass]
    public class ContentClientNotificationHandlerTests
    {
        [TestMethod]
        public void OnDataReceived_TypeAsContentData_ReturnsValidContentData()
        {
            // send message and deserialize it from fake notification handler
            var mockHelper = new MockHelper();
            ChatData chatData = mockHelper.GenerateChatData(MessageType.Chat, MessageEvent.New, "This is a message string", messageID: 6, replyThreadID: 2);
            IContentSerializer serializer = new ContentSerializer();
            IContentClient chatHandler = ContentClientFactory.GetInstance();
            var mockNotificationHandler = new MockContentNotificationHandler(chatHandler);
            string serializedData = serializer.Serialize(chatData);

            mockNotificationHandler.OnDataReceived(serializedData);
            ChatData dataRecevied = mockNotificationHandler.GetReceivedMessage();

            Assert.AreEqual(chatData.Type, dataRecevied.Type);
            Assert.AreEqual(chatData.Data, dataRecevied.Data);
            Assert.AreEqual(chatData.ReplyThreadID, dataRecevied.ReplyThreadID);
            Assert.AreEqual(chatData.FileData, dataRecevied.FileData);
            Assert.AreEqual(chatData.Starred, dataRecevied.Starred);
            Assert.AreEqual(chatData.Event, dataRecevied.Event);
        }

        [TestMethod]
        public void OnDataReceived_InvalidObjectType_ReturnsArgumentException()
        {
            // send an int object (which is not supported)
            var mockhelper = new MockHelper();
            int data = 0;
            IContentSerializer serializer = new ContentSerializer();
            IContentClient contentHandler = ContentClientFactory.GetInstance();
            var mockNotificationHandler = new MockContentNotificationHandler(contentHandler);
            string serializedData = serializer.Serialize(data);

            Assert.ThrowsException<ArgumentException>(() => mockNotificationHandler.OnDataReceived(serializedData));
        }
    }
}
