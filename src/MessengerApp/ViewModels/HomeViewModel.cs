using MessengerApp.Stores;
using MessengerApp.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerDashboard;

namespace MessengerApp.ViewModels
{
    internal class HomeViewModel : ViewModelBase
    {
        public string WelcomeMessage => "Welcome To the home page";
        public string ?UserName { get;}

        public string ?UserImage { get; }
        public ICommand NavigateMeetCommand { get; }

        public HomeViewModel(NavigationStore navigationStore)
        {
            UserName = navigationStore.AuthResult.UserName;
            UserImage = navigationStore.AuthResult.UserImage;
            
            NavigateMeetCommand = new NavigateMeetCommand(navigationStore);
        }
    }
}
