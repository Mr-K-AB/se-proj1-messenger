/******************************************************************************
* Filename    = ContentDB.cs
*
* Author      = 
*
* Product     = Messenger
* 
* Project     = MessengerContent
*
* Description = 
* *****************************************************************************/

using MessengerContent.DataModels;

namespace MessengerContent.Server
{
    public class ContentDB
    {
        private readonly List<ChatThread> _chatThread;
        private readonly Dictionary<int, int> _chatIdToDataMap;
        private readonly Dictionary<int, ChatData> _filesMap;

        /// <summary>
        ///     Database Constructor to initialize member variables.
        /// </summary>
        public ContentDB()
        {
            _filesMap = new Dictionary<int, ChatData>();
            _chatThread = new List<ChatThread>();
            _chatIdToDataMap = new Dictionary<int, int>();
            IdGenerator.ResetChatId();
            IdGenerator.ResetMsgId();
        }

        /// <summary>
        ///     Fetch all the chat threads
        /// </summary>
        public List<ChatThread> GetChatThreads()
        {
            return _chatThread;
        }

        /// <summary>
        ///     Function to store Messages on Database.
        /// </summary>
        public ChatData MessageStore(ChatData msg)
        {
            msg.MessageID = IdGenerator.GetMsgId();
            // If message is a part of already existing chatThread, add to the thread
            if (_chatIdToDataMap.ContainsKey(msg.ReplyThreadID))
            {
                int threadIndex = _chatIdToDataMap[msg.ReplyThreadID];
                ChatThread chatThread = _chatThread[threadIndex];
                ReceiveChatData message = msg.Copy();
                chatThread.AddMessage(message);
            }
            // else create a new chatContext and add the message to it
            else
            {
                var chatThread = new ChatThread();
                int newThreadId = IdGenerator.GetChatId();
                msg.ReplyThreadID = newThreadId;
                ReceiveChatData message = msg.Copy();
                chatThread.AddMessage(message);

                _chatThread.Add(chatThread);
                //Decrease the count by 1, because we had already incremented the count.
                _chatIdToDataMap[chatThread.ThreadID] = _chatThread.Count - 1;
            }

            return msg;
        }

        /// <summary>
        ///     Retrieve message from the Database based on the thread ID and message ID 
        /// </summary>
        public ReceiveChatData GetMessage(int threadId, int _msgId)
        {
            // If given ChatThread or Message doesn't exists return null
            if (!_chatIdToDataMap.ContainsKey(threadId))
            {
                return null;
            }

            int threadIndex = _chatIdToDataMap[threadId];
            ChatThread chat = _chatThread[threadIndex];

            // If given ChatContext doesn't contain the message return null
            if (!chat.ContainsMessageID(_msgId))
            {
                return null;
            }

            int messageIndex = chat.GetMessageIndex(_msgId);
            return chat.MessageList[messageIndex];
        }

        /// <summary>
        ///     Function to store Files on Database.
        /// </summary>
        public ChatData FileStore(ChatData msg)
        {
            ChatData message = MessageStore(msg);
            _filesMap[message.MessageID] = msg;
            return message;
        }

        /// <summary>
        ///     Function to Fetch the stored file with a given id from the database.
        /// </summary>
        public ChatData? FilesFetch(int _msgId)
        {
            // If requested messageId is not in the map return null
            return !_filesMap.ContainsKey(_msgId) ? null : _filesMap[_msgId];
        }
    }
}
