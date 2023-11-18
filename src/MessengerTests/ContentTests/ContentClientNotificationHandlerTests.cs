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
            var mockhelper = new MockHelper();
            var chatData = MockHelper.GenerateChatData(MessageType.Chat, MessageEvent.New, "This is a message string", messageID: 6, replyThreadID: 2);
            IContentSerializer serializer = new ContentSerializer();
            var chatHandler = ContentClientFactory.GetInstance();
            var mockNotificationHandler = new MockContentNotificationHandler(chatHandler);
            var serializedData = serializer.Serialize(chatData);

            mockNotificationHandler.OnDataReceived(serializedData);
            var dataRecevied = mockNotificationHandler.GetReceivedMessage();

            Assert.IsType<ChatData>(dataRecevied);
            Assert.Equal(chatData.Type, dataRecevied.Type);
            Assert.Equal(chatData.Data, dataRecevied.Data);
            Assert.Equal(chatData.ReceiverIDs, dataRecevied.ReceiverIDs);
            Assert.Equal(chatData.ReplyThreadID, dataRecevied.ReplyThreadID);
            Assert.Equal(chatData.FileData, dataRecevied.FileData);
            Assert.Equal(chatData.Starred, dataRecevied.Starred);
            Assert.Equal(chatData.Event, dataRecevied.Event);
        }

        [TestMethod]
        public void OnDataReceived_InvalidObjectType_ReturnsArgumentException()
        {
            // send an int object (which is not supported)
            var mockhelper = new MockHelper();
            int data = 0;
            IContentSerializer serializer = new ContentSerializer();
            var contentHandler = ContentClientFactory.GetInstance();
            var mockNotificationHandler = new MockContentNotificationHandler(contentHandler);
            var serializedData = serializer.Serialize(data);

            Assert.Throws<ArgumentException>(() => mockNotificationHandler.OnDataReceived(serializedData));
        }
    }
}
