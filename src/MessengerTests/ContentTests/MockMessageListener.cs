/******************************************************************************
 * Filename    = FakeContentListener.cs
 *
 * Author      = Rapeti Siddhu Neehal
 *
 * Product     = Messenger
 * 
 * Project     = Messenger Content
 *
 * Description = Class that mocks the content listener
 *****************************************************************************/

using MessengerContent.DataModels;
using Messenger.Client;
using MessengerContent.Client;

namespace MessengerTests.ContentTests
{
    public class MockMessageListener : IMessageListener
    {
        // content listener parameters
        private ReceiveChatData _receivedMessage;
        private List<ChatThread> _allMessages;
        /// <summary>
        /// Constructor to create content listener
        /// </summary>
        public MockMessageListener()
        {
            _receivedMessage = new ReceiveChatData();
            _allMessages = new List<ChatThread>();
        }

        ///<inheritdoc/>
        public void OnMessageReceived(ReceiveChatData messageData)
        {
            _receivedMessage = messageData;

        }

        /// <summary>
        /// Gets the received message
        /// </summary>
        /// <returns>Received message</returns>
        public ReceiveChatData GetReceivedMessage()
        {
            return _receivedMessage;
        }

        public void OnAllMessagesReceived(List<ChatThread> allMessages)
        {
            _allMessages = allMessages;
        }
        /// <summary>
        /// Gets the list of threads containing all messages
        /// </summary>
        /// <returns>List of threads containing all messages</returns>
        public List<ChatThread> GetAllMessages()
        {
            return _allMessages;
        }
    }
}
