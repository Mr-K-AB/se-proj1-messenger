using MessengerApp.Stores;
using MessengerApp.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MessengerApp.ViewModels
{
    internal class HomeViewModel : ViewModelBase
    {
        public string WelcomeMessage => "Welcome To the home page";
        public ICommand NavigateMeetCommand { get; }

        public HomeViewModel(NavigationStore navigationStore)
        {
            NavigateMeetCommand = new NavigateMeetCommand(navigationStore);
        }
    }
}
