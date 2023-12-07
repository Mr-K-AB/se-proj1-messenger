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

using System.Windows.Input;
using MessengerViewModels.Commands;
using MessengerViewModels.Stores;

namespace MessengerViewModels.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public string WelcomeMessage => "Welcome To the home page";

        public string? UserName { get; }

        public string? UserImage { get; }

        public string? UserEmail { get; }

        public string? JoinMeetIP { get; set; }
        public int JoinMeetPort { get; set; }
        public ICommand NavigateServerMeetCommand { get; }

        public ICommand NavigateClientMeetCommand { get; }
        public HomeViewModel(NavigationStore navigationStore)
        {
            UserName = navigationStore.AuthResult.UserName;
            UserImage = navigationStore.AuthResult.UserImage;
            UserEmail = navigationStore.AuthResult.UserEmail;

            NavigateServerMeetCommand = new NavigateServerMeetCommand(navigationStore);
            NavigateClientMeetCommand = new NavigateClientMeetCommand(navigationStore, this);
        }
    }
}
