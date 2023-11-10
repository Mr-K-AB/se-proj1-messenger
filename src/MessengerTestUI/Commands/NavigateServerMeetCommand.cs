using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerTestUI.Stores;
using MessengerTestUI.ViewModels;
using MessengerDashboard;

namespace MessengerTestUI.Commands
{
    internal class NavigateServerMeetCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        public NavigateServerMeetCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }
        public override async void Execute(object parameter)
        {
            _navigationStore.CurrentViewModel = new ServerMeetViewModel(_navigationStore);
        }
    }
}
