using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Messenger.Client;
using MessengerContent.Client;
using MessengerContent.DataModels;
using MessengerContent;

namespace MessengerApp.ViewModels
{

    public class ChatPageViewModel : INotifyPropertyChanged, IMessageListener
    {
        /// <summary>
        ///     Content Client Data Model
        /// </summary>
        private readonly IContentClient _model;

        /// <summary>
        ///     Dictionary mapping Messages IDs to their ThreadIds
        /// </summary>
        public IDictionary<int, int> ThreadIds;


        /// <summary>
        ///     Constructor for ViewModel
        /// </summary>
        /// <param name="production">true for production mode</param>
        public ChatPageViewModel()
        {
            Trace.WriteLine("[ChatPageViewModel] ViewModel setup");
            ThreadIds = new Dictionary<int, int>();
        }

        /// <summary>
        ///     Message to be sent
        /// </summary>
        public SendChatData MsgToSend { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnAllMessagesReceived(List<ChatThread> allMessages)
        {
            throw new System.NotImplementedException();
        }

        public void OnMessageReceived(ReceiveChatData contentData)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///     Sends the message to content module. Message type is determined by messageType parameter
        /// </summary>
        /// <param name="message"> The string containing the file path </param>
        /// <param name="replyMsgId"> Either the reply ID of the mesage being replied to or -1 denoting its not a reply message </param>
        /// <param name="messageType"> File or Chat </param>
        public void SendMessage(string message, int replyMsgId, string messageType)
        {

            // Creating a SendContentData object
            MsgToSend = new SendChatData();
            // Setting message type field
            if (messageType == "File")
            {
                MsgToSend.Type = MessageType.File;
            }
            else if (messageType == "Chat")
            {
                MsgToSend.Type = MessageType.Chat;
            }

            // Setting the remaining fields of the SendContentData object
            MsgToSend.ReplyMessageID = replyMsgId;
            MsgToSend.Data = message;
            MsgToSend.ReplyThreadID = replyMsgId != -1 ? ThreadIds[replyMsgId] : -1;

            // Empty list denotes it's broadcast message
            MsgToSend.ReceiverIDs = new int[] { };

            if (messageType == "File")
            {
                Trace.WriteLine("[ChatPageViewModel] I am Sending a File Message");
            }
            else if (messageType == "Chat")
            {
                Trace.WriteLine("[ChatPageViewModel] I am Sending a Chat Message");
            }
            _model.ClientSendData(MsgToSend);
        }
    }
}
