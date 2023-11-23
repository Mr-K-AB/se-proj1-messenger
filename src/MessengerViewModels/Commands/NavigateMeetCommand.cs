using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerViewModels.Stores;
using MessengerViewModels.ViewModels;
using MessengerDashboard;

namespace MessengerViewModels.Commands
{
    public class NavigateMeetCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        public NavigateMeetCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }
        public override async void Execute(object parameter)
        {
            _navigationStore.CurrentViewModel = new MeetViewModel(_navigationStore);
        }
    }
}
