/******************************************************************************
* Filename    = HostMeetingView.xaml.cs
*
* Author      = Geddam Gowtham
*
* Roll Number = 112001011
*
* Product     = Messenger 
* 
* Project     = MessengerApp
*
* Description = Interaction logic for Meeting View for host .
* *****************************************************************************/
using System.Windows;
using System.Windows.Controls;
using TraceLogger;

namespace MessengerApp.Views
{
    /// <summary>
    /// Interaction logic for HostMeetingView.xaml
    /// </summary>
    public partial class HostMeetingView : UserControl
    {
        public HostMeetingView()
        {
            InitializeComponent();
            ChatBubble chatBubbleControl = new();
            OverlayContent.Content = chatBubbleControl;
        }

        private void Chat_Click(object sender, RoutedEventArgs e)
        {

            Logger.Debug("[HostMeetingView] On chat_click, ChatPanel changes");
            if (ChatPanel.Width == new GridLength(0))
            {
                ChatPanel.Width = new GridLength(300);
            }
            else
            {
                ChatPanel.Width = new GridLength(0);
            }
            //OverlayPanel.Visibility = OverlayPanel.IsVisible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
