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
using MessengerContent.DataModels;

namespace PlexShareTests.ContentTests
{
    public class FakeContentListener : IMessageListener
    {
        // content listener parameters
        private ReceiveChatData _receivedMessage;

        /// <summary>
        /// Constructor to create content listener
        /// </summary>
        public FakeContentListener()
        {
            _receivedMessage = new ReceiveChatData();
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
    }
}
