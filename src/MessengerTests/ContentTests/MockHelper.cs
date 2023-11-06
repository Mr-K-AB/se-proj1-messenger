/******************************************************************************
 * Filename    = MockHelper.cs
 *
 * Author      = Rapeti Siddhu Neehal
 *
 * Product     = Messenger
 * 
 * Project     = MessengerTests
 *
 * Description = Mock Class containing helper functions for unit testing
 *****************************************************************************/
using MessengerContent;
using MessengerContent.DataModels;
using MessengerContent.Enums;

namespace MessengerTests.ContentTests
{
    public class MockHelper
    {
        private readonly FakeCommunicator _fakeCommunicator;

        /// <summary>
        /// Constructor to instantiate fake communicator
        /// </summary>
        public MockHelper()
        {
            _fakeCommunicator = new FakeCommunicator();
        }

        /// <summary>
        /// Gets the fake communicator instance
        /// </summary>
        /// <returns></returns>
        public FakeCommunicator GetFakeCommunicator()
        {
            return _fakeCommunicator;
        }

        /// <summary>
        /// Generates an object of the SendChatData class 
        /// </summary>
        /// <param name="data">Message string</param>
        /// <param name="receiverIDs">List of receiver IDs</param>
        /// <param name="replyThreadID">ID of thread the message belongs to</param>
        /// <param name="type">Type of message - Chat or File</param>
        /// <returns></returns>
        public SendChatData GenerateSendChatData(
            MessageType type = MessageType.Chat,
            string data = "This is a sample message",
            int[] receiverIDs = null,
            int replyThreadID = -1,
            int replyMessageID = -1
        )
        {
            receiverIDs ??= Array.Empty<int>();
            var sendChatData = new SendChatData
            {
                Data = data,
                ReceiverIDs = receiverIDs,
                ReplyThreadID = replyThreadID,
                Type = type,
                ReplyMessageID = replyMessageID
            };
            return sendChatData;
        }

        /// <summary>
        /// Generates an object of the ReceiveChatData class 
        /// </summary>
        /// <param name="type">Type of message - Chat or File</param>
        /// <param name="event">Message event - New, Edit, Delete, etc.</param>
        /// <param name="data">Message string</param>
        /// <param name="messageID">ID of the message</param>
        /// <param name="receiverIDs">List of receiver IDs</param>
        /// <param name="replyThreadID">ID of thread the message belongs to</param>
        /// <param name="senderID">ID of the sender</param>
        /// <param name="starred">Boolean for starred message</param>
        /// <returns>Object of the ReceiveChatData class</returns>
        public ReceiveChatData GenerateReceiveChatData(
            MessageType type = MessageType.Chat,
            MessageEvent @event = MessageEvent.New,
            string data = "This is a sample message",
            int messageID = -1,
            int[] receiverIDs = null,
            int replyThreadID = -1,
            int senderID = -1,
            bool starred = false,
            int replyMessageID = -1
        )
        {
            receiverIDs ??= new int[0];
            var receiveChatData = new ReceiveChatData
            {
                Event = @event,
                Data = data,
                MessageID = messageID,
                ReceiverIDs = receiverIDs,
                SenderID = senderID,
                ReplyThreadID = replyThreadID,
                Starred = starred,
                Type = type,
                ReplyMessageID = replyMessageID
            };
            return receiveChatData;
        }

        /// <summary>
        /// Generates an object of the ChatData class 
        /// </summary>
        /// <param name="type">Type of message - Chat or File</param>
        /// <param name="event">Message event - New, Edit, Delete, etc.</param>
        /// <param name="data">Message string</param>
        /// <param name="messageID">ID of the message</param>
        /// <param name="receiverIDs">List of receiver IDs</param>
        /// <param name="replyThreadID">ID of thread the message belongs to</param>
        /// <param name="senderID">ID of the sender</param>
        /// <param name="starred">Boolean for starred message</param>
        /// <returns>Object of the ChatData class</returns>
        public ChatData GenerateChatData(
            MessageType type = MessageType.Chat,
            MessageEvent @event = MessageEvent.New,
            string data = "This is a sample message",
            int messageID = -1,
            int[] receiverIDs = null,
            int replyThreadID = -1,
            int senderID = -1,
            bool starred = false,
            int replyMessageID = -1
        )
        {
            receiverIDs ??= Array.Empty<int>();
            var newChatData = new ChatData
            {
                Event = @event,
                Data = data,
                MessageID = messageID,
                ReceiverIDs = receiverIDs,
                SenderID = senderID,
                ReplyThreadID = replyThreadID,
                Starred = starred,
                Type = type,
                ReplyMessageID = replyMessageID
            };
            return newChatData;
        }

        /// <summary>
        /// Checks if two ReceiveChatData objects are equal
        /// </summary>
        /// <param name="rcdata1">ReceiveChatData object</param>
        /// <param name="rcdata2">ReceiveChatData objeect</param>
        public void CheckReceiveChatData(ReceiveChatData rcdata1, ReceiveChatData rcdata2)
        {
            Assert.AreEqual(rcdata1.Type, rcdata2.Type);
            Assert.AreEqual(rcdata1.Data, rcdata2.Data);
            Assert.AreEqual(rcdata1.MessageID, rcdata2.MessageID);
            Assert.AreEqual(rcdata1.ReceiverIDs, rcdata2.ReceiverIDs);
            Assert.AreEqual(rcdata1.SenderID, rcdata2.SenderID);
            Assert.AreEqual(rcdata1.ReplyMessageID, rcdata2.ReplyMessageID);
            Assert.AreEqual(rcdata1.ReplyThreadID, rcdata2.ReplyThreadID);
            Assert.AreEqual(rcdata1.SenderID, rcdata2.SenderID);
            Assert.AreEqual(rcdata1.Starred, rcdata2.Starred);
            Assert.AreEqual(rcdata1.Event, rcdata2.Event);
        }

    }


}
