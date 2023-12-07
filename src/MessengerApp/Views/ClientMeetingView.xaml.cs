/******************************************************************************
* Filename    = ClientMeetingView.xaml.cs
*
* Author      = Geddam Gowtham
*
* Roll Number = 112001011
*
* Product     = Messenger 
* 
* Project     = MessengerApp
*
* Description = Interaction logic for Meeting View for client .
* *****************************************************************************/
using System;
using System.Collections.Generic;
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
using MessengerDashboard;
using MessengerDashboard.Client;
using MessengerDashboard.Client.Events;
using TraceLogger;

namespace MessengerApp.Views
{
    /// <summary>
    /// Interaction logic for ClientMeetingView.xaml
    /// </summary>
    public partial class ClientMeetingView : UserControl
    {
        private IClientSessionController _dashboard => DashboardFactory.GetClientSessionController();

        public ClientMeetingView()
        {
            InitializeComponent();
            ChatBubble chatBubbleControl = new();
            OverlayContent.Content = chatBubbleControl;
            if (_dashboard.SessionInfo.SessionMode == SessionMode.Exam)
            {
                OverlayContent.Visibility = Visibility.Collapsed;
            }
            _dashboard.SessionChanged += HandleSessionViewChange;

        }

        private void HandleSessionViewChange(object? sender, ClientSessionChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.Session.SessionMode == SessionMode.Exam)
                {
                    ChatPanel.Width = new GridLength(0);

                    OverlayPanel.Visibility = Visibility.Collapsed;
                    Logger.Debug("[ClientMeetingView] Exam mode, chat grid visibility is collapsed");
                }
                else
                {
                    OverlayPanel.Visibility = Visibility.Visible;
                    OverlayContent.Visibility = Visibility.Visible;
                }

            });




        }

        //private void Dashboard_Click(object sender, RoutedEventArgs e)
        //{
        //    DashboardControl dashboardControl = new();
        //    MainContent.Content = dashboardControl;
        //}

        //private void WhiteBoard_Click(object sender, RoutedEventArgs e)
        //{
        //    WhiteboardControl whiteboardControl = new(1);
        //    MainContent.Content = whiteboardControl;
        //}

        //private void ClientScreenshare_Click(object sender, RoutedEventArgs e)
        //{
        //    ScreenshareClientControl screenshareClientControl = new();
        //    MainContent.Content = screenshareClientControl;
        //}

        private void Chat_Click(object sender, RoutedEventArgs e)
        {
            if (_dashboard.SessionInfo.SessionMode == SessionMode.Lab)
            {
                OverlayPanel.Visibility = Visibility.Visible;
                OverlayContent.Visibility = Visibility.Visible;
                if (ChatPanel.Width == new GridLength(0))
                {
                    ChatPanel.Width = new GridLength(300);
                    Logger.Debug($"[ClientMeetingView] Chat clicked. In Lab mode, chatpanel expanded.");
                }
                else
                {
                    ChatPanel.Width = new GridLength(0);
                    Logger.Debug($"[ClientMeetingView] Chat clicked. In Lab mode, chatpanel collapsed.");
                }
            }
            //OverlayPanel.Visibility = OverlayPanel.IsVisible ? Visibility.Collapsed : Visibility.Visible;
        }


    }
}
