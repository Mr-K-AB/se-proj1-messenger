/******************************************************************************
* Filename    = HomeView.xaml.cs
*
* Author      = Geddam Gowtham
*
* Roll Number = 112001011
*
* Product     = Messenger 
* 
* Project     = MessengerApp
*
* Description = Interaction logic for View after authentication - HomeView.
* *****************************************************************************/
using System.Windows;
using System.Windows.Controls;
using MessengerViewModels.ViewModels;
using TraceLogger;

namespace MessengerApp.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        private readonly MeetingView _meetingView = new();
        public HomeView()
        {
            InitializeComponent();
            MainContent.Content = _meetingView;
        }

        private void Meeting_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _meetingView;
            Logger.Debug("[HomeView] On Meeting_Click, main content changed to meetingview usercontrol");
        }

        private void Session_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is HomeViewModel homeViewModel)
            {
                MainContent.Content = new SessionControl(homeViewModel.UserEmail);
            }
        }
    }
}
