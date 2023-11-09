using MessengerTestUI.Stores;
using MessengerTestUI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerDashboard;

namespace MessengerTestUI.ViewModels
{
    internal class HomeViewModel : ViewModelBase
    {
        public string WelcomeMessage => "Welcome To the home page";
        public string ?UserName { get;}

        public string ?UserImage { get; }

        public string ? JoinMeetIP {  get; set; }
        public int JoinMeetPort { get; set; }
        public ICommand NavigateServerMeetCommand { get; }

        public HomeViewModel(NavigationStore navigationStore)
        {
            UserName = navigationStore.AuthResult.UserName;
            UserImage = navigationStore.AuthResult.UserImage;

            NavigateServerMeetCommand = new NavigateServerMeetCommand(navigationStore);
        }
    }
}
