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
using MessengerViewModels.ViewModels;

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
