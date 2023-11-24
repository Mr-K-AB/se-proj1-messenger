/******************************************************************************
* Filename    = HomeViewModel.cs
*
* Author      = Vinay Ingle
*
* Roll Number = 112001050
*
* Product     = Messenger 
* 
* Project     = ViewModels
*
* Description = ViewModel for Home View after authentication.
* *****************************************************************************/

using MessengerViewModels.Stores;
using MessengerViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerDashboard;

namespace MessengerViewModels.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public string WelcomeMessage => "Welcome To the home page";
        public string? UserName { get; }

        public string? UserImage { get; }

        public string? JoinMeetIP { get; set; }
        public int JoinMeetPort { get; set; }
        public ICommand NavigateServerMeetCommand { get; }

        public ICommand NavigateClientMeetCommand { get; }
        public HomeViewModel(NavigationStore navigationStore)
        {
            UserName = navigationStore.AuthResult.UserName;
            UserImage = navigationStore.AuthResult.UserImage;

            NavigateServerMeetCommand = new NavigateServerMeetCommand(navigationStore);
            NavigateClientMeetCommand = new NavigateClientMeetCommand(navigationStore, this);
        }
    }
}
