/******************************************************************************
 * Filename    = IMessageListener.cs
 *
 * Author      = Rapeti Siddhu Neehal
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContent
 *
 * Description = Interface for message listener
 *****************************************************************************/

using MessengerContent.DataModels;
using System.Collections.Generic;

namespace Messenger.Client
{
    public interface IMessageListener
    {
        /// <summary>
        /// Handles the reception of a message.
        /// </summary>
        /// <param name="chatData">Instance of ReceiveChatData class</param>
        void OnMessageReceived(ReceiveChatData chatData);

        /// <summary>
        /// Handles event of all messages sent to / received from client at once
        /// </summary>
        /// <param name="allMessages">List of thread objects containing all messages</param>
        void OnAllMessagesReceived(List<ChatThread> allMessages);
    }
}
