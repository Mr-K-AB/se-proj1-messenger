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

        /// <summary>
        ///     All the messages upto now
        /// </summary>
        private readonly ObservableCollection<ChatMessage> _allMessages;
        private readonly ChatMessage _addNewMessage;
        public ChatBubble()
        {
            InitializeComponent();

            var viewModel = new ChatPageViewModel(true);

            // Subscribed to the Property Changed Event
            viewModel.PropertyChanged += Listener;
            DataContext = viewModel;

            // Binding all the messages
            _allMessages = new ObservableCollection<ChatMessage>();
            MainChat.ItemsSource = _allMessages;
        }

        /// <summary>
        ///     Replied message's Message ID 
        /// </summary>
        public int ReplyMsgId { get; set; }

        /// <summary>
        ///     Property Changed Event in which the view gets updated with new messages
        /// </summary>
        /// <param name="sender"> Sender Notifying the event </param>
        /// <param name="e"> Property Changed Event </param>
        private void Listener(object sender, PropertyChangedEventArgs e)
        {
            string? propertyName = e.PropertyName;
            var viewModel = DataContext as ChatPageViewModel;

            if (propertyName == "ReceivedMsg")
            {
                // Adding the new message to our collection(_allMessage) which in turn adds a chat bubble in the UX listbox
                _allMessages.Add(viewModel.ReceivedMsg);
                Trace.WriteLine("[ChatPageView] New message has been added.");
                UpdateScrollBar(MainChat);
            }
            else if (propertyName == "ReceivedAllMsgs")
            {
                // Adding all the messages for the new user from the session to our collection(_allMessage) which in turn adds a chat bubble in the UX listbox
                _allMessages.Add(viewModel.ReceivedMsg);
                Trace.WriteLine("[ChatPageView] Restoring session chat messages.");
                UpdateScrollBar(MainChat);
            }
            else if (propertyName == "EditOrDelete")
            {
                ChatMessage updatedMsg = null;
                string replyMsgOld = "";
                string replyMsgNew = "";
                // We find the Message in our Observable Collection containing all the messages and update this entry
                // We store its text message in replyMsgOld for later updating the ReplyMessage of the messages containing this text message
                for (int i = 0; i < _allMessages.Count; i++)
                {
                    ChatMessage message = _allMessages[i];
                    if (message.MessageID == viewModel.ReceivedMsg.MessageID)
                    {
                        replyMsgOld = message.IncomingMessage;
                        replyMsgNew = viewModel.ReceivedMsg.IncomingMessage;
                        ChatMessage toUpd = new()
                        {
                            isCurrentUser = message.isCurrentUser,
                            ReplyMessage = message.ReplyMessage,
                            Sender = message.Sender,
                            Time = message.Time,
                            MessageID = message.MessageID,
                            MessageType = message.MessageType,
                            IncomingMessage = viewModel.ReceivedMsg.IncomingMessage
                        };
                        
                        // updating
                        _allMessages[i] = toUpd;
                        updatedMsg = _allMessages[i];
                    }
                }

                // Updating all the Chat bubbles which all have replied to the message that has been Edited/Deleted
                for (int i = 0; i < _allMessages.Count; i++)
                {
                    ChatMessage message = _allMessages[i];
                    if (message.ReplyMessage == replyMsgOld)
                    {
                        ChatMessage toUpd = new()
                        {
                            isCurrentUser = message.isCurrentUser,
                            ReplyMessage = replyMsgNew,
                            Sender = message.Sender,
                            Time = message.Time,
                            MessageID = message.MessageID,
                            MessageType = message.MessageType,
                            IncomingMessage = message.IncomingMessage
                        };
                        
                        // updating
                        _allMessages[i] = toUpd;
                        updatedMsg = _allMessages[i];
                    }
                }
                Trace.WriteLine($"[ChatPageView] Message ID {updatedMsg.MessageID} was updated with {updatedMsg.IncomingMessage}.");
            }
        }
        /// <summary>
        ///     Event Handler upon clicking upload button to send file
        /// </summary>
        /// <param name="sender"> Notification Sender </param>
        /// <param name="e"> Routed Event Data </param>
        private void UploadButtonClick(object sender, RoutedEventArgs e)
        {
            if (ReplyTextBox.Text == string.Empty)
            {
                var viewModel = DataContext as ChatPageViewModel;

                // Create OpenFileDialog
                var openFileDialog = new OpenFileDialog();

                // Launch OpenFileDialog by calling ShowDialog method
                var result = openFileDialog.ShowDialog();
                var size = new FileInfo(openFileDialog.FileName).Length;
                // Process open file dialog box results
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
                    Trace.WriteLine($"[ChatPageView] {openFileDialog.SafeFileName} File sent for uploading from view.");

                    // Clearing the TextBoxes
                    SendTextBox.Text = string.Empty;
                    ReplyTextBox.Text = string.Empty;
                }
            }
        }

        /// <summary>
        ///     Event Handler on Clicking Send Button
        /// </summary>
        /// <param name="sender"> Notification Sender </param>
        /// <param name="e"> Routed Event Data </param>
        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            string msg = SendTextBox.Text;
            msg = msg.Trim();
            // We send a message only when the text box is not empty
            if (!string.IsNullOrEmpty(msg))
            {
                // Character limit set to avoid long paragraphs
                if (msg.Length > 300)
                {
                    MessageBox.Show("Please enter less than 300 characters!");
                    return;
                }
                var viewModel = DataContext as ChatPageViewModel;

                // If ReplyTextBox is not empty, that means we are replying to a message and we shall pass the corresponding reference ReplyMsgId
                if (string.IsNullOrEmpty(ReplyTextBox.Text))
                {
                    viewModel.SendMessage(msg, -1, "Chat");
                }
                else
                {
                    viewModel.SendMessage(msg, ReplyMsgId, "Chat");
                }
                Trace.WriteLine("[ChatPageView] Sending a message from view.");

                // Clearing the TextBoxes
                SendTextBox.Text = string.Empty;
                ReplyTextBox.Text = string.Empty;
            }
        }

        /// <summary>
        ///     Event Handler on Clicking Reply Button
        /// </summary>
        /// <param name="sender"> Notification Sender</param>
        /// <param name="e"> Routed Event Data </param>
        private void ReplyButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton)
            {
                ///Button senderButton = (Button)sender;
                if (senderButton.DataContext is ChatMessage msg)
                {
                    ///ChatMessage msg = (ChatMessage)senderButton.DataContext;
                    if (msg.IncomingMessage != "Message deleted.")
                    {
                        string? message = msg.IncomingMessage;
                        if (message.Length > 10)
                        {
                            message = message.Substring(0, 10);
                            message += "...";
                        }
                        string senderBox = msg.Sender + ": " + message;
                        ReplyTextBox.Text = senderBox;
                        ReplyMsgId = msg.MessageID;
                        Trace.WriteLine("[ChatPageView] Reply button clicked.");
                    }
                }
            }
        }


        /// <summary>
        ///     Event Handler on Clicking Edit Button
        /// </summary>
        /// <param name="sender"> Notification Sender</param>
        /// <param name="e"> Routed Event Data </param>
        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SendTextBox.Text))
            {
                if (sender is Button senderButton)
                {
                    ///var senderButton = (Button)sender;
                    if (senderButton.DataContext is ChatMessage msg)
                    {
                        var viewModel = DataContext as ChatPageViewModel;
                        ///ChatMessage msg = (ChatMessage)senderButton.DataContext;
                        if (msg.IncomingMessage != "Message Deleted.")
                        {
                            string? ourEditMessage = SendTextBox.Text;
                            viewModel.EditChatMsg(msg.MessageID, ourEditMessage);
                            SendTextBox.Text = string.Empty;
                            Trace.WriteLine("[ChatPageView] Message has been deleted.");
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Event Handler on Clicking Delete Button
        /// </summary>
        /// <param name="sender"> Notification Sender</param>
        /// <param name="e"> Routed Event Data </param>
        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton)
            {
                ///Button senderButton = (Button)sender;
                if (senderButton.DataContext is ChatMessage msg)
                {
                    var viewModel = DataContext as ChatPageViewModel;
                    ///ChatMessage msg = (ChatMessage)senderButton.DataContext;
                    viewModel.DeleteChatMsg(msg.MessageID);
                    Trace.WriteLine("[ChatPageView] Delete button clicked.");
                }
            }
        }

        /// <summary>
        ///     Event Handler on Clearing Reply TextBlock
        /// </summary>
        /// <param name="sender"> Notification Sender</param>
        /// <param name="e"> Routed Event Data </param>
        private void ClearReplyBox(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                ///Button senderButton = (Button)sender;
                ReplyTextBox.Text = null;
                Trace.WriteLine("[ChatPageView] Reply text box cleared.");
            }
        }

        /// <summary>
        ///     Event Handler for Clicking on Star Radio Button
        /// </summary>
        /// <param name="sender"> Notification Sender </param>
        /// <param name="e"> Routed Event Data </param>
        private void StarButtonClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ChatPageViewModel;
            if (sender is RadioButton senderRadioButton)
            {
                ///RadioButton senderRadioButton = (RadioButton)sender;

                if (senderRadioButton.DataContext is ChatMessage msg)
                {
                    ///ChatMessage msg = (ChatMessage)senderRadioButton.DataContext;
                    viewModel.StarChatMsg(msg.MessageID);
                    Trace.WriteLine("[ChatPageView] Message has been starred.");
                }

            }

        }

        /// <summary>
        ///     Event Handler on clicking Download Button
        /// </summary>
        /// <param name="sender"> Notification Sender </param>
        /// <param name="e"> Routed Event Data </param>
        private void DownloadButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton)
            {
                var viewModel = DataContext as ChatPageViewModel;

                // Creating a SaveFileDialog
                SaveFileDialog dailogFile = new ();

                ///Button senderButton = (Button)sender;
                if (senderButton.DataContext is ChatMessage message)
                {
                    // Getting the message through download button
                    ///ChatMessage message = (ChatMessage)senderButton.DataContext;

                    // Set the default file name and extension
                    dailogFile.FileName = System.IO.Path.GetFileNameWithoutExtension(message.IncomingMessage);
                    dailogFile.DefaultExt = System.IO.Path.GetExtension(message.IncomingMessage);

                    // Display save file dialog box
                    bool? result = dailogFile.ShowDialog();

                    // if Download OK
                    if (result == true)
                    {
                        viewModel.DownloadFile(dailogFile.FileName, message.MessageID);
                        Trace.WriteLine("[ChatPageView] Download button clicked.");
                    }
                }
            }
        }

        /// <summary>
        ///     Updates the Scrollbar to the bottom of the listbox
        /// </summary>
        /// <param name="listBox"> Listbox containing the scrollbar </param>
        private void UpdateScrollBar(ListBox listBox)
        {
            if ((listBox != null) && (VisualTreeHelper.GetChildrenCount(listBox) != 0))
            {
                var border = (Border)VisualTreeHelper.GetChild(listBox, 0);
                var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
                Trace.WriteLine("[ChatPageView] ScrollBar Updated.");
            }
        }

    }
}
