using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerApp.Stores;
using MessengerApp.ViewModels;
using MessengerDashboard;

namespace MessengerApp.Commands
{
    internal class NavigateMeetCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        public NavigateMeetCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }
        public override async void Execute(object parameter)
        {
            _navigationStore.CurrentViewModel = new HomeViewModel(_navigationStore);
        }
    }
}
