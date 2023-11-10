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
    internal class NavigateDashboardCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        //private readonly string 

        public NavigateDashboardCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }

        public override void Execute(object parameter)
        {
            _navigationStore.SubViewModel = new DashboardViewModel();
        }
    }
}
