/******************************************************************************
* Filename    = ChatPageViewModel.cs
*
* Author      = M V Nagasurya
*
* Product     = Messenger
* 
* Project     = MessengerApp
*
* Description = View Model for ChatBubble
*****************************************************************************/

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
using TraceLogger;

namespace MessengerApp.ViewModels
{

    /// <summary>
    /// Chat Page View Model class
    /// </summary>
    public class ChatPageViewModel : INotifyPropertyChanged, IUserNotification, IMessageListener
    {
        // Client Model
        private readonly IContentClient _model;

        public IDictionary<int, string> UserIdToNames; // Mapping User IDS to their names

        public IDictionary<int, string> Messages; // Mapping Message IDs to their context

        public IDictionary<int, int> ThreadIds; // Mapping Message IDs to their ThreadIDs

        // For testing. It is true if testing mode is enabled, else false
        public bool testingMode;

        // For testing the received Messages
        public List<ChatMessage> TestChatMessages;

        /// <summary>
        ///     Constructor for ViewModel
        /// </summary>
        public ChatPageViewModel(bool testing = false)
        {
            UserIdToNames = new Dictionary<int, string>();
            Messages = new Dictionary<int, string>();
            ThreadIds = new Dictionary<int, int>();
            testingMode = testing;

            if (testing)
            {
                TestChatMessages = new();
            }

            _model = ContentClientFactory.GetInstance();
            _model.ClientSubscribe(this);
        }

        /// <summary>
        ///     The current user id
        /// </summary>
        public static int UserId { get; private set; }

        /// <summary>
        ///  The current username
        /// </summary>
        public static string UserName { get; private set; }

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
        /// <param name="property"> The property which is changed </param>
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        ///     Sends the message to content module
        /// </summary>
        /// <param name="message"> Message Data </param>
        /// <param name="replyMsgId"> Reply Message Id </param>
        /// <param name="messageType"> Type of message </param>
        public void SendMessage(string message, int replyMsgId, string messageType)
        {
            Logger.Log("[ChatPageViewModel] Message to be sent, received", LogLevel.INFO);
            MsgToSend = new SendChatData();

            if (messageType == "File")
            {
                MsgToSend.Type = MessageType.File;
            }
            else if (messageType == "Chat")
            {
                MsgToSend.Type = MessageType.Chat;
            }
            else { throw new Exception("MessageType Undefined!"); }

            MsgToSend.ReplyMessageID = replyMsgId;
            MsgToSend.Data = message;
            MsgToSend.ReplyThreadID = -1;
            if (replyMsgId != -1) // If the message is a reply to some other msg
            {
                MsgToSend.ReplyThreadID = ThreadIds[replyMsgId];
            }
            if (!testingMode)
            {
                UserId = _model.GetUserID();
                UserName = _model.GetUserName();
                _model.ClientSendData(MsgToSend);
            }
        }

        /// <summary>
        ///     Download a file at the required location in the client's machine using Path
        /// </summary>
        /// <param name="savePath"> Path of File </param>
        /// <param name="msgId"> Message ID  </param>
        public void DownloadFile(string savePath, int msgId)
        {
            Logger.Log("[ChatPageViewModel] Message need to be downloaded received", LogLevel.INFO);
            _model.ClientDownload(msgId, savePath);
        }

        /// <summary>
        ///     Updating the message Data of Message ID with the New Message
        /// </summary>
        /// <param name="msgID"> Message ID </param>
        /// <param name="newMsg"> The updated Chat Message  </param>
        public void EditChatMsg(int msgID, string newMsg)
        {
            Logger.Log("[ChatPageViewModel] Edited Message received", LogLevel.INFO);
            _model.ClientEdit(msgID, newMsg);
        }

        /// <summary>
        ///     Delete Messages using msgID
        /// </summary>
        /// <param name="msgID"> Message ID </param>
        public void DeleteChatMsg(int msgID)
        {
            Logger.Log("[ChatPageViewModel] Delete Message received", LogLevel.INFO);
            _model.ClientDelete(msgID);
        }

        /// <summary>
        ///     Star Messages
        /// </summary>
        /// <param name="msgId"> The Starred Message ID </param>
        public void StarChatMsg(int msgId)
        {
            Logger.Log("[ChatPageViewModel] Starred Message received", LogLevel.INFO);
            _model.ClientStar(msgId);
        }

        private Dispatcher ApplicationMainThreadDispatcher =>
            (Application.Current?.Dispatcher != null) ?
                    Application.Current.Dispatcher :
                    Dispatcher.CurrentDispatcher;

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
                        Logger.Log("[ChatPageViewModel] Users List Received.", LogLevel.INFO);
                        UserIdToNames.Clear();
                        foreach (UserInfo user in currentSession.Users)
                        {
                            // adding users for the session
                            UserIdToNames.Add(user.UserId, user.UserName);
                        }
                    }
                }
            }),
            currentSession);
        }


        /// <summary>
        ///     Handles the appropriate event on the received message
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
                        Logger.Log("[ChatPageViewModel] New Message has been received", LogLevel.INFO);
                        Messages.Add(contentData.MessageID, contentData.Data);
                        ThreadIds.Add(contentData.MessageID, contentData.ReplyThreadID);

                        // Create a new ChatMessage Instance for the new Received Msg
                        //if(contentData.ReplyMessageID != -1)
                        //{
                        //    if (Messages[contentData.ReplyMessageID] == null) 
                        //    {
                        //        Messages[contentData.ReplyMessageID] = "Message does not exist";
                        //    }
                        //}
                        ReceivedMsg = new()
                        {
                            MessageID = contentData.MessageID,
                            MessageType = contentData.Type == MessageType.Chat,
                            MsgData = Path.GetFileName(contentData.Data),
                            Time = contentData.SentTime.ToString("hh:mm tt"),
                            Sender = senderName,
                            isCurrentUser = UserId == contentData.SenderID,
                            ReplyMessage = contentData.ReplyMessageID == -1 ? null : Messages[contentData.ReplyMessageID],
                            ReplyMessageId = contentData.ReplyMessageID
                        };

                        OnPropertyChanged("NewReceivedMsg"); // Raise PropertChangedEvent
                    }
                    else if (contentData.Event == MessengerContent.Enums.MessageEvent.Edit || contentData.Event == MessengerContent.Enums.MessageEvent.Delete)
                    {

                        // Creating a new ChatMessage instance for ReceivedMsg
                        // For both the Edit and Delete events, the implementation when the message is received is same. 
                        // The Deleted Messages should always have MsgData as "Message Deleted."
                        // The Edited Messages will have MsgData as some text given by the user in the TextBox.
                        if (contentData.Event == MessengerContent.Enums.MessageEvent.Edit)
                        {
                            Logger.Log("[ChatPageViewModel] Edited Message has been received", LogLevel.INFO);
                        }
                        else if (contentData.Event == MessengerContent.Enums.MessageEvent.Delete)
                        {
                            Logger.Log("[ChatPageViewModel] Deleted Message has been received", LogLevel.INFO);
                        }
                        ReceivedMsg = new()
                        {
                            MessageID = contentData.MessageID,
                            MessageType = contentData.Type == MessageType.Chat,
                            MsgData = contentData.Data,
                            Time = contentData.SentTime.ToString("hh:mm tt"),
                            Sender = senderName,
                            isCurrentUser = UserId == contentData.SenderID,
                            ReplyMessage = contentData.ReplyMessageID == -1 ? null : Messages[contentData.ReplyMessageID],
                            ReplyMessageId = contentData.ReplyMessageID
                        };
                        Messages[contentData.MessageID] = ReceivedMsg.MsgData;

                        OnPropertyChanged("Edited/Deleted"); // Raise PropertChangedEvent
                    }
                    else if (contentData.Event == MessengerContent.Enums.MessageEvent.Star)
                    {
                        Logger.Log("[ChatPageViewModel] Starred Message has been received", LogLevel.INFO);
                        ReceivedMsg = new()
                        {
                            MessageID = contentData.MessageID,
                            MessageType = contentData.Type == MessageType.Chat,
                            MsgData = contentData.Data,
                            Time = contentData.SentTime.ToString("hh:mm tt"),
                            Sender = senderName,
                            isCurrentUser = UserId == contentData.SenderID,
                            ReplyMessage = contentData.ReplyMessageID == -1 ? null : Messages[contentData.ReplyMessageID],
                            ReplyMessageId = contentData.ReplyMessageID
                        };
                        Messages[contentData.MessageID] = ReceivedMsg.MsgData;
                        if(contentData.Starred)
                        {
                            OnPropertyChanged("Starred"); // Raise PropertChangedEvent
                        }
                        else
                        {
                            OnPropertyChanged("NotStarred"); // Raise PropertChangedEvent
                        }
                    }
                }
            }),
            contentData);
        }

        /// <summary>
        ///     When a new user joins, they receive the list of messages upto then
        /// </summary>
        /// <param name="chatHistory"> List of all messages upto now </param>
        public void OnAllMessagesReceived(List<ChatThread> chatHistory)
        {
            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                      DispatcherPriority.Normal,
            new Action<List<ChatThread>>(chatHistory =>
            {
                lock (this)
                {
                    Messages.Clear();
                    ThreadIds.Clear();
                    // updating the Threads and Messages dictionary and displaying the chat upto now in the listbox in view
                    Logger.Log("[ChatPageViewModel] Received All Messages", LogLevel.INFO);
                    foreach (ChatThread messageList in chatHistory)
                    {
                        foreach (ReceiveChatData message in messageList.MessageList)
                        {

                            Messages.Add(message.MessageID, message.Data);
                            ThreadIds.Add(message.MessageID, message.ReplyThreadID);

                            if (!testingMode) { UserId = _model.GetUserID(); }
                            else // for testing
                            {
                                UserId = 1;
                            }

                            ReceivedMsg = new()
                            {
                                MessageID = message.MessageID,
                                MessageType = message.Type == MessageType.Chat,
                                MsgData = message.Data,
                                Time = message.SentTime.ToString("hh:mm tt"),
                                Sender = message.SenderName,
                                isCurrentUser = UserId == message.SenderID,
                                ReplyMessage = message.ReplyMessageID == -1 ? null : Messages[message.ReplyMessageID],
                                ReplyMessageId = message.ReplyMessageID
                            };
                            if (testingMode)
                            {
                                TestChatMessages.Add(ReceivedMsg);
                            }
                            OnPropertyChanged("HistoryMsgs");
                        }
                    }
                }
            }),
            chatHistory);
        }
    }
}
