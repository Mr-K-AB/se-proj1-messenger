using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Messenger.Client;
using MessengerContent.Client;
using MessengerContent.DataModels;
using MessengerContent;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerApp.ViewModels;
using System;
using System.Windows.Threading;
using MessengerDashboard;
using MessengerDashboard.Dashboard;
using MessengerDashboard.Dashboard.User.Session;
using System.Windows;
using MessengerDashboard.Client;

namespace MessengerApp.ViewModels
{


    public class ChatPageViewModel : IMessageListener, INotifyPropertyChanged, IUserNotification
    {
        //Data Models
        private readonly IContentClient _model;

        private readonly IUXUserSessionManager _modelDb;

        public IDictionary<int, string> Users; // Mapping User IDS to their names

        public IDictionary<int, string> Messages; // Mapping Message IDs to their context

        public IDictionary<int, int> ThreadIds; // Mapping Message IDs to their ThreadIDs

        /// <summary>
        ///     Constructor for ViewModel
        /// </summary>
        public ChatPageViewModel()
        {
            Users = new Dictionary<int, string>();
            Messages = new Dictionary<int, string>();
            ThreadIds = new Dictionary<int, int>();

            _model = ContentClientFactory.GetInstance();
            _model.ClientSubscribe(this);

        }

        /// <summary>
        ///     The current user id
        /// </summary>
        public static int UserId { get; private set; }
        
        /// <summary>
        ///     The received message
        /// </summary>
        public ChatMessage ReceivedMsg { get; private set; }

        /// <summary>
        ///     Message to be sent
        /// </summary>
        public SendChatData MsgToSend { get; private set; }


        /// <summary>
        ///     A PropertyChangedEvent is raised whenever a property is changed
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        ///     Handling the raised PropertyChangedEvent
        /// </summary>
        /// <param name="property"> </param>
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        ///     Sends the message to content module
        /// </summary>
        /// <param name="message"> </param>
        /// <param name="replyMsgId">  </param>
        /// <param name="messageType">  </param>
        public void SendMessage(string message, int replyMsgId, string messageType)
        {
            MsgToSend = new SendChatData();
            
            if (messageType == "File")
            {
                MsgToSend.Type = MessageType.File;
            }
            else if (messageType == "Chat")
            {
                MsgToSend.Type = MessageType.Chat;
            }

            MsgToSend.ReplyMessageID = replyMsgId;
            MsgToSend.Data = message;
            MsgToSend.ReplyThreadID = -1;
            if (replyMsgId != -1) // If the message is a reply to some other msg
            {
                MsgToSend.ReplyThreadID = ThreadIds[replyMsgId];
            }

            MsgToSend.ReceiverIDs = new int[] { }; // Empty list denotes it's broadcast message

            _model.ClientSendData(MsgToSend);
        }

        /// <summary>
        ///     Download a file at the required location in the client's machine using Path
        /// </summary>
        /// <param name="savePath"> </param>
        /// <param name="msgId">  </param>
        public void DownloadFile(string savePath, int msgId)
        {
            _model.ClientDownload(msgId, savePath);
        }

        /// <summary>
        ///     Updating the message Data of Message ID with the New Message
        /// </summary>
        /// <param name="msgID"> Message ID </param>
        /// <param name="newMsg"> The updated Chat Message  </param>
        public void EditChatMsg(int msgID, string newMsg)
        {
            _model.ClientEdit(msgID, newMsg);
        }

        /// <summary>
        ///     Delete Messages using msgID
        /// </summary>
        /// <param name="msgID"> </param>
        public void DeleteChatMsg(int msgID)
        {
            _model.ClientDelete(msgID);
        }

        /// <summary>
        ///     Star Messages
        /// </summary>
        /// <param name="msgId"> </param>
        public void StarChatMsg(int msgId)
        {
            _model.ClientStar(msgId);
        }

        public void OnUserSessionChange(SessionInfo session)
        {
            throw new NotImplementedException();
        }
    }
}
