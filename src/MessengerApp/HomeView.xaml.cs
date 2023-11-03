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

namespace MessengerApp
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : Page
    {
        public HomeView()
        {
            InitializeComponent();
        }
        private void MeetingMenuItem_Click( object sender , RoutedEventArgs e )
        {
            // Create an instance of the MeetingView or load the content you want to display for meetings.
            MeetingView meetingView = new MeetingView();
            MainContent.Content = meetingView;
        }

        private void OldSessionsMenuItem_Click( object sender , RoutedEventArgs e )
        {
            // Create an instance of the OldSessionsView or load the content for old sessions.
            OldSessionsView oldSessionsView = new OldSessionsView();
            MainContent.Content = oldSessionsView;
        }

    }
}
