/******************************************************************************
* Filename    = ChatBubble.xaml.cs
*
* Author      = M V Nagasurya
*
* Product     = Messenger
* 
* Project     = MessengerApp
*
* Description = Helper functions
*****************************************************************************/


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessengerApp.ViewModels;
using MessengerContent.DataModels;
using Microsoft.Win32;

namespace MessengerApp
{
    /// <summary>
    /// Interaction logic for ChatBubble.xaml
    /// </summary>
    public partial class ChatBubble : UserControl
    {

        private readonly ObservableCollection<ChatMessage> _msgCollection; // All the messages uptil now
        public ChatBubble()
        {
            InitializeComponent();

            var viewModel = new ChatPageViewModel();
            viewModel.PropertyChanged += Listener; // Subscribe to PropertyChangedEvent
            DataContext = viewModel;

            _msgCollection = new ObservableCollection<ChatMessage>();
            
            MainChat.ItemsSource = _msgCollection; // Binding all the messages to the MainChat (ListBox)
        }

        /// <summary>
        ///     Replied message's Message ID 
        /// </summary>
        public int ReplyMsgId
        {
            get;
            set;
        }

        /// <summary>
        ///     Updates the display with new messages
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void Listener(object? sender, PropertyChangedEventArgs e)
        {

            string? propertyName = e.PropertyName; // Changed Property Name

            var viewModel = DataContext as ChatPageViewModel;

            if (propertyName == "ReceivedMsg")
            {
                _msgCollection.Add(viewModel.ReceivedMsg); // Adding the received message into the collection (_msgCollection)
                UpdateScrollBar(MainChat);
            }
            else if (propertyName == "ReceivedAllMsgs")
            {
                // Adding all the messages for the new user to our collection(_msgCollection)
                _msgCollection.Add(viewModel.ReceivedMsg);
                UpdateScrollBar(MainChat);
            }
            else if (propertyName == "EditOrDelete")
            {
                string replyMsg = "";
                string replyMsgUpd = ""; // for storing the edited message

                for (int i = 0; i < _msgCollection.Count; i++) // find the msg which has been triggered
                {
                    ChatMessage message = _msgCollection[i];
                    if (message.MessageID == viewModel.ReceivedMsg.MessageID)
                    {
                        replyMsg = message.MsgData;
                        replyMsgUpd = viewModel.ReceivedMsg.MsgData;
                        ChatMessage toUpd = new()
                        {
                            isCurrentUser = message.isCurrentUser,
                            ReplyMessage = message.ReplyMessage,
                            Sender = message.Sender,
                            Time = message.Time,
                            MessageID = message.MessageID,
                            MessageType = message.MessageType,
                            MsgData = viewModel.ReceivedMsg.MsgData
                        };

                        _msgCollection[i] = toUpd; // Updating the incoming message
                    }
                }

                //Find all the messages which have replied to the modified message, and change that message in that bubble
                for (int i = 0; i < _msgCollection.Count; i++)
                {
                    ChatMessage message = _msgCollection[i];
                    if (message.ReplyMessage == replyMsg)
                    {
                        ChatMessage toUpd = new()
                        {
                            isCurrentUser = message.isCurrentUser,
                            ReplyMessage = replyMsgUpd,
                            Sender = message.Sender,
                            Time = message.Time,
                            MessageID = message.MessageID,
                            MessageType = message.MessageType,
                            MsgData = message.MsgData
                        };

                        _msgCollection[i] = toUpd; // updating all the messages which replied to the message which is modified
                    }
                }
            }
            return;
        }


        /// <summary>
        ///     Handler for the Send Button
        /// </summary>
        /// <param name="sender"> Notification Sender </param>
        /// <param name="e"> Routed Event Data </param>
        private void SendHandler(object sender, RoutedEventArgs e)
        {
            string msg = SendTextBox.Text;
            //MessageBox.Show("hello");
            msg = msg.Trim();

            if (!string.IsNullOrEmpty(msg)) // Character Limit in the TextBox
            {
                if (msg.Length > 300)
                {
                    MessageBox.Show("Please enter less than 300 characters!");
                    return;
                }
                var viewModel = DataContext as ChatPageViewModel;


                if (string.IsNullOrEmpty(ReplyTextBox.Text))
                {
                    viewModel.SendMessage(msg, -1, "Chat");
                }
                else // If the replyTextBox has some msg, then pass the MsgId
                {
                    viewModel.SendMessage(msg, ReplyMsgId, "Chat");
                }

                SendTextBox.Text = string.Empty; // clear the textbox
                ReplyTextBox.Text = string.Empty;
            }
            return;
        }

        /// <summary>
        ///     Handler for the ReplyButton
        /// </summary>
        /// <param name="sender"> Notification Sender</param>
        /// <param name="e"> Routed Event Data </param>
        private void ReplyHandler(object sender, RoutedEventArgs e)
        {
            if ((sender is Button senderButton) && (senderButton.DataContext is ChatMessage msg))
            {

                string? message = msg.MsgData;
                if (message != "Message deleted.") // Can't reply to the deleted messages
                {
                    if (message.Length > 10) // Showing only the short view of the msg
                    {
                        message = message.Substring(0, 10);
                        message += "...";
                    }
                    string senderBox = msg.Sender + ": " + message;
                    ReplyTextBox.Text = senderBox;
                    ReplyMsgId = msg.MessageID;
                }
            }
            return;
        }

        /// <summary>
        ///     Handler for edit button
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void EditHandler(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SendTextBox.Text)) // Only if there is some text in the textbox, we can edit the msg.
            {
                if ((sender is Button senderButton) && (senderButton.DataContext is ChatMessage msg))
                {
                    var viewModel = DataContext as ChatPageViewModel;
                    if (msg.MsgData != "Message Deleted.") // can't edit deleted message
                    {
                        string? EditedMsg = SendTextBox.Text;
                        viewModel.EditChatMsg(msg.MessageID, EditedMsg);
                        SendTextBox.Text = string.Empty;
                    }
                }
            }
            return;
        }

        /// <summary>
        ///     Handler for the delete button
        /// </summary>
        /// <param name="sender"> Notification Sender</param>
        /// <param name="e"> Routed Event Data </param>
        private void DeleteHandler(object sender, RoutedEventArgs e)
        {
            if ((sender is Button senderButton) && (senderButton.DataContext is ChatMessage msg))
            {

                var viewModel = DataContext as ChatPageViewModel;
                viewModel.DeleteChatMsg(msg.MessageID);
            }
        }

        /// <summary>
        ///     Handler for Clearing reply box
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void UndoReplyHandler(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                ReplyTextBox.Text = null;
            }
            return;
        }

        /// <summary>
        ///     Upload File Button Handler
        /// </summary>
        /// <param name="sender"> Notification Sender </param>
        /// <param name="e"> Routed Event Data </param>
        private void UploadHandler(object sender, RoutedEventArgs e)
        {
            if (ReplyTextBox.Text == string.Empty)
            {
                var viewModel = DataContext as ChatPageViewModel;

                var openFileDialog = new OpenFileDialog();

                bool? result = openFileDialog.ShowDialog();
                long size; // size of the file
                try
                {
                    size = new FileInfo(openFileDialog.FileName).Length;
                }
                catch // If the user didnt select any file
                {
                    MessageBox.Show("Choose a File!");
                    return;
                }

                if (result == true)
                {
                    if (size > 10240000)
                    {
                        MessageBox.Show("File size is greater than 10MB!");
                        return;
                    }
                    if (string.IsNullOrEmpty(ReplyTextBox.Text))
                    {
                        viewModel.SendMessage(openFileDialog.FileName, -1, "File");
                    }
                    else
                    {
                        viewModel.SendMessage(openFileDialog.FileName, ReplyMsgId, "File");
                    }

                    SendTextBox.Text = string.Empty; // after the message has been sent, clearing the textbox
                    ReplyTextBox.Text = string.Empty;
                }
            }
        }

        /// <summary>
        ///     Download Button Event Handler on Click
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void DownloadHandler(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderBtn)
            {
                var viewModel = DataContext as ChatPageViewModel;

                SaveFileDialog dailogFile = new();

                if (senderBtn.DataContext is ChatMessage message)
                {
                    // Set the File name and the Extension of the file from the message
                    dailogFile.FileName = System.IO.Path.GetFileNameWithoutExtension(message.MsgData);
                    dailogFile.DefaultExt = System.IO.Path.GetExtension(message.MsgData);

                    bool? isDownloadOk = dailogFile.ShowDialog();

                    if (isDownloadOk == true)
                    {
                        viewModel.DownloadFile(dailogFile.FileName, message.MessageID);
                    }
                }
            }
            return;
        }

        /// <summary>
        ///     Handler for Star Radio Button
        /// </summary>
        /// <param name="sender">  </param>
        /// <param name="e"> </param>
        private void StarHandler(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ChatPageViewModel;
            if ((sender is RadioButton senderRadioBtn) && (senderRadioBtn.DataContext is ChatMessage msg))
            {
                viewModel.StarChatMsg(msg.MessageID);
            }
        }


        /// <summary>
        ///     Updates the Scrollbar
        /// </summary>
        /// <param name="listBox"> </param>
        private void UpdateScrollBar(ListBox listBox)
        {
            if ((listBox != null) && (VisualTreeHelper.GetChildrenCount(listBox) != 0))
            {
                var border = (Border)VisualTreeHelper.GetChild(listBox, 0);
                var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
            return;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

    }
}
