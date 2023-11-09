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
    internal class NavigateHomeCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        //private readonly string 

        public NavigateHomeCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }

        public override void Execute(object parameter)
        {
            _navigationStore.CurrentViewModel = new HomeViewModel(_navigationStore);
        }
    }
}
