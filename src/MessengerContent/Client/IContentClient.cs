using Messenger.Client;
using MessengerContent.DataModels;

namespace MessengerContent.Client
{
    public interface IContentClient
    {
        public void SetUser(int id, string name, string ip, int port);
        /// <summary>
        /// Sends chat or file data to clients
        /// </summary>
        /// <param name="chatData">Instance of SendChatData class</param>
        void ClientSendData(SendChatData chatData, string ip, int port);

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
        void ClientEdit(int messageID, string newMessage, string ip, int port);

        /// <summary>
        /// Delete a previous chat message
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        void ClientDelete(int messageID, string ip, int port);

        /// <summary>
        /// Download file to specific path on client machine
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <param name="savePath">Path to which the file will be downloaded</param>
        void ClientDownload(int messageID, string savePath, string ip, int port);

        /// <summary>
        /// Star message for it to be included in the dashboard summary
        /// </summary>
        /// <param name="messageID"></param>
        void ClientStar(int messageID, string ip, int port);

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

        string GetUserName();

        string GetIP();

        int GetPort();
    }
}
