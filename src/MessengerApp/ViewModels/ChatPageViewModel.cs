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
using System.Windows;
using MessengerDashboard.Client;
using SharpDX.Direct3D11;
using MessengerApp.DataModel;

namespace MessengerApp.ViewModels
{


    public class ChatPageViewModel :  INotifyPropertyChanged, IUserNotification, IMessageListener
    {
        //Data Models
        private readonly IContentClient _model;

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

        public static string UserName { get; private set; }

        public static string ServerIP { get; private set; }
        
        public static int ServerPort { get; private set; }
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
            UserId = _model.GetUserID();
            UserName = _model.GetUserName();
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


        public void OnUserSessionChange(SessionInfo currentSession)
        {
            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                      DispatcherPriority.Normal,
                      new Action<SessionInfo>(currentSession =>
                      {
                          lock (this)
                          {
                              if (currentSession != null)
                              {
                                  Trace.WriteLine("[ChatPageViewModel] Users List Received.");
                                  Users.Clear();
                                  foreach (UserInfo user in currentSession.Users)
                                  {
                                      // adding users for the session
                                      Users.Add(user.UserId, user.UserName);
                                  }
                              }
                          }
                      }),
                      currentSession);
        }

        private Dispatcher ApplicationMainThreadDispatcher =>
            (Application.Current?.Dispatcher != null) ?
                    Application.Current.Dispatcher :
                    Dispatcher.CurrentDispatcher;

        /// <summary>
        ///     Handles the appropriate even on the received message
        /// </summary>
        /// <param name="contentData"> </param>
        public void OnMessageReceived(ReceiveChatData contentData)
        {
            int senderID = contentData.SenderID;
            string? senderName = contentData.SenderName;
            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                      DispatcherPriority.Normal,
                      new Action<ReceiveChatData>(contentData =>
                      {
                          lock (this)
                          {
                              if (contentData.Event == MessengerContent.Enums.MessageEvent.New)
                              {
                                  Trace.WriteLine("[ChatPageViewModel] New Message has been received.");
                                  Messages.Add(contentData.MessageID, contentData.Data);
                                  ThreadIds.Add(contentData.MessageID, contentData.ReplyThreadID);

                                  ReceivedMsg = new()
                                  {

                                      MessageID = contentData.MessageID,
                                      MessageType = contentData.Type == MessageType.Chat,
                                      MsgData = Path.GetFileName(contentData.Data),
                                      Time = contentData.SentTime.ToString("hh:mm tt"),
                                      Sender = senderName,
                                      isCurrentUser = UserId == contentData.SenderID,
                                      ReplyMessage = contentData.ReplyMessageID == -1 ? "" : Messages[contentData.ReplyMessageID]
                                  };

                                  OnPropertyChanged("ReceivedMsg");
                              }
                              else if (contentData.Event == MessengerContent.Enums.MessageEvent.Edit || contentData.Event == MessengerContent.Enums.MessageEvent.Delete)
                              {

                                  // Creating object for the received message
                                  // Message object, ReceivedMsg, will modify the current user's _allmessages list upon property changed event
                                  ReceivedMsg = new()
                                  {
                                      MessageID = contentData.MessageID,
                                      MessageType = contentData.Type == MessageType.Chat,
                                      MsgData = contentData.Data,
                                      Time = contentData.SentTime.ToString("hh:mm tt"),
                                      Sender = senderName,
                                      //Sender = contentData.SenderID,
                                      isCurrentUser = UserId == contentData.SenderID,
                                      ReplyMessage = contentData.ReplyMessageID == -1 ? "" : Messages[contentData.ReplyMessageID],
                                  };
                                  Messages[contentData.MessageID] = ReceivedMsg.MsgData;

                                  OnPropertyChanged("EditOrDelete");
                              }
                          }
                      }),
                      contentData);
        }

        /// <summary>
        ///     When a new user joins, they receive the list of messages upto then
        /// </summary>
        /// <param name="allMessages"> List of all messages upto now </param>
        public void OnAllMessagesReceived(List<ChatThread> allMessages)
        {
            // Execute the call on the application's main thread.
            //
            // Also note that we may execute the call asynchronously as the calling
            // thread is not dependent on the callee thread finishing this method call.
            // Hence we may call the dispatcher's BeginInvoke method which kicks off
            // execution async as opposed to Invoke which does it synchronously.

            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                      DispatcherPriority.Normal,
                      new Action<List<ChatThread>>(allMessages =>
                      {
                          lock (this)
                          {
                              // clearing existing data entries
                              Messages.Clear();
                              ThreadIds.Clear();
                              // updating the Threads and Messages dictionary and displaying the chat upto now in the listbox in view
                              foreach (ChatThread messageList in allMessages)
                              {
                                  foreach (ReceiveChatData message in messageList.MessageList)
                                  {
                                      Trace.WriteLine("[ChatPageViewModel] All messages have been received.");
                                      Messages.Add(message.MessageID, message.Data);
                                      ThreadIds.Add(message.MessageID, message.ReplyThreadID);

                                      UserId = _model.GetUserID();

                                      // Creating object for the received message
                                      // Message object, ReceivedMsg, to be added to the new user's _allmessages
                                      // list upon property changed event
                                      ReceivedMsg = new()
                                      {
                                          MessageID = message.MessageID,
                                          MessageType = message.Type == MessageType.Chat,
                                          MsgData = message.Data,
                                          Time = message.SentTime.ToString("hh:mm tt"),
                                          Sender = Users.ContainsKey(message.SenderID) ? Users[message.SenderID] : "Anonymous",
                                          isCurrentUser = UserId == message.SenderID,
                                          ReplyMessage = message.ReplyMessageID == -1 ? "" : Messages[message.ReplyMessageID]
                                      };

                                      // Propery Changed Event raised for updating View with current session's chat
                                      OnPropertyChanged("ReceivedAllMsgs");
                                  }
                              }
                          }
                      }),
                      allMessages);
        }
    }
}

