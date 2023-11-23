/******************************************************************************
 * Filename    = IContentClient.cs
 *
 * Author      = Rapeti Siddhu Neehal
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContent
 *
 * Description = Interface for ContentClient
 *****************************************************************************/

using Messenger.Client;
using MessengerContent.DataModels;

namespace MessengerContent.Client
{
    public interface IContentClient
    {
        public void SetUser(int id, string name);
        /// <summary>
        /// Sends chat or file data to clients
        /// </summary>
        /// <param name="chatData">Instance of SendChatData class</param>
        void ClientSendData(SendChatData chatData);

        /// <summary>
        /// Lets client subscribe to notifications from this class
        /// </summary>
        /// <param name="subscriber">Subscriber object which is an implementation of the INotificationListener interface</param>
        void ClientSubscribe(IMessageListener subscriber);

        /// <summary>
        /// Edit a previous chat message
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <param name="newMessage">Edited message</param>
        void ClientEdit(int messageID, string newMessage);

        /// <summary>
        /// Delete a previous chat message
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        void ClientDelete(int messageID);

        /// <summary>
        /// Download file to specific path on client machine
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <param name="savePath">Path to which the file will be downloaded</param>
        void ClientDownload(int messageID, string savePath);

        /// <summary>
        /// Star message for it to be included in the dashboard summary
        /// </summary>
        /// <param name="messageID"></param>
        void ClientStar(int messageID);

        /// <summary>
        /// Get message thread corresponding to thread ID
        /// </summary>
        /// <param name="threadID">ID of the thread</param>
        /// <returns>Object implementing ChatThread class</returns>
        ChatThread ClientGetThread(int threadID);

        /// <summary>
        /// Get user ID associated with instance
        /// </summary>
        /// <returns>User ID associated with instance</returns>
        int GetUserID();

        /// <summary>
        /// Get user name associated with instance
        /// </summary>
        /// <returns>User name associated with instance</returns>
        string GetUserName();
    }
}
