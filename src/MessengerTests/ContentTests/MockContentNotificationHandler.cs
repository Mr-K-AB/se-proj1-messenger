/******************************************************************************
 * Filename    = MockContentNotificationHandler.cs
 *
 * Author      = Rapeti siddhu neehal
 *
 * Product     = Messenger
 * 
 * Project     = MessengerTests
 *
 * Description = Mock Notification handler that handles received data from server   
 *****************************************************************************/

using MessengerContent.Client;
using MessengerContent.DataModels;

namespace MessengerTests.ContentTests
{
    public class MockContentNotificationHandler : ContentClientNotificationHandler
    {
        // member varialbes to store received message and all messages
        private ChatData _contentData;
        private List<ChatThread> _chatThreads;

        /// <summary>
        /// constructor that calls base class ContentClient constructor
        /// </summary>
        /// <param name="contentClient"></param>
        public MockContentNotificationHandler(IContentClient contentClient) : base(contentClient)
        {
            _contentData = new ChatData();
            _chatThreads = new List<ChatThread>();
        }

        /// <summary>
        /// Reset member variables
        /// </summary>
        private void Reset()
        {
            _contentData = new ChatData();
            _chatThreads = new List<ChatThread>();
        }

        /// <summary>
        /// Gets the received message 
        /// </summary>
        /// <returns>Object of ContentData class</returns>
        public ChatData GetReceivedMessage()
        {
            Reset();
            _contentData = _receivedMessage;
            return _contentData;
        }

        /// <summary>
        /// Gets list of chat threads containing all messages
        /// </summary>
        /// <returns>List of chat threads</returns>
        public List<ChatThread> GetAllMessages()
        {
            Reset();
            _chatThreads = _allMessages;
            return _chatThreads;
        }
    }
}
