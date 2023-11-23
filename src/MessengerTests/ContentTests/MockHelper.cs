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
        private readonly MockCommunicator _mockCommunicator;

        public MockHelper()
        {
            _mockCommunicator = new MockCommunicator();
        }

        public MockCommunicator GetMockCommunicator()
        {
            return _mockCommunicator;
        }

        public SendChatData GenerateSendChatData(
            MessageType type = MessageType.Chat,
            string data = "This is a sample message",
            int replyThreadID = -1,
            int replyMessageID = -1
        )
        {
            var sendChatData = new SendChatData
            {
                Data = data,
                ReplyThreadID = replyThreadID,
                Type = type,
                ReplyMessageID = replyMessageID
            };
            return sendChatData;
        }

        public ReceiveChatData GenerateReceiveChatData(
            MessageType type = MessageType.Chat,
            MessageEvent @event = MessageEvent.New,
            string data = "This is a sample message",
            int messageID = -1,
            int replyThreadID = -1,
            int senderID = -1,
            bool starred = false,
            int replyMessageID = -1
        )
        {
            var receiveChatData = new ReceiveChatData
            {
                Event = @event,
                Data = data,
                MessageID = messageID,
                SenderID = senderID,
                ReplyThreadID = replyThreadID,
                Starred = starred,
                Type = type,
                ReplyMessageID = replyMessageID
            };
            return receiveChatData;
        }

        public ChatData GenerateChatData(
            MessageType type = MessageType.Chat,
            MessageEvent @event = MessageEvent.New,
            string data = "This is a sample message",
            int messageID = -1,
            int replyThreadID = -1,
            int senderID = -1,
            bool starred = false,
            int replyMessageID = -1
        )
        {
            var newChatData = new ChatData
            {
                Event = @event,
                Data = data,
                MessageID = messageID,
                SenderID = senderID,
                ReplyThreadID = replyThreadID,
                Starred = starred,
                Type = type,
                ReplyMessageID = replyMessageID
            };
            return newChatData;
        }

        public void CheckReceiveChatData(ReceiveChatData rcdata1, ReceiveChatData rcdata2)
        {
            Assert.AreEqual(rcdata1.Type, rcdata2.Type);
            Assert.AreEqual(rcdata1.Data, rcdata2.Data);
            Assert.AreEqual(rcdata1.MessageID, rcdata2.MessageID);
            Assert.AreEqual(rcdata1.SenderID, rcdata2.SenderID);
            Assert.AreEqual(rcdata1.ReplyMessageID, rcdata2.ReplyMessageID);
            Assert.AreEqual(rcdata1.ReplyThreadID, rcdata2.ReplyThreadID);
            Assert.AreEqual(rcdata1.SenderID, rcdata2.SenderID);
            Assert.AreEqual(rcdata1.Starred, rcdata2.Starred);
            Assert.AreEqual(rcdata1.Event, rcdata2.Event);
        }

        /// <summary>
        /// Checks if two chat threads are similar
        /// </summary>
        /// <param name="thread1">ChatThread object</param>
        /// <param name="thread2">ChatThread object</param>
        public void CheckChatThreads(ChatThread thread1, ChatThread thread2)
        {
            Assert.AreEqual(thread1.ThreadID, thread2.ThreadID);
            Assert.AreEqual(thread1.MessageList.Count, thread2.MessageList.Count);
            for (int i = 0; i < thread1.MessageList.Count; i++)
            {
                CheckReceiveChatData(thread1.MessageList[i], thread2.MessageList[i]);
            }
        }

        /// <summary>
        /// Checks if two lists of chat threads are similar
        /// </summary>
        /// <param name="list1">List of chat threads</param>
        /// <param name="list2">List of chat threads</param>
        public void CheckChatThreadLists(List<ChatThread> list1, List<ChatThread> list2)
        {
            Assert.AreEqual(list1.Count, list2.Count);
            for (int i = 0; i < list1.Count; i++)
            {
                CheckChatThreads(list1[i], list2[i]);
            }
        }
    }
}
