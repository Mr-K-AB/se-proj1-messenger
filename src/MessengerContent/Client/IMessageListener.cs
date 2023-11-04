
using MessengerContent.DataModels;
using System.Collections.Generic;

namespace Messenger.Client
{
    public interface IMessageListener
    {
        /// <summary>
        /// Handles the reception of a message.
        /// </summary>
        /// <param name="contentData">Instance of ReceiveContentData class</param>
        void OnMessageReceived(ReceiveChatData contentData);

        /// <summary>
        /// Handles event of all messages sent to / received from client at once
        /// </summary>
        /// <param name="allMessages">List of thread objects containing all messages</param>
        void OnAllMessagesReceived(List<ChatThread> allMessages);
    }
}
