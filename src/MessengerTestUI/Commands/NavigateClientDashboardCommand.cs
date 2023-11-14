using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerTestUI.Stores;
using MessengerTestUI.ViewModels;
using MessengerDashboard;
using System.Diagnostics;

namespace MessengerTestUI.Commands
{
    internal class NavigateClientDashboardCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        //private readonly string 

        public NavigateClientDashboardCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }

        public override void Execute(object parameter)
        {
            _navigationStore.SubViewModel = new DashboardClientViewModel(_navigationStore);
        }
    }
}
