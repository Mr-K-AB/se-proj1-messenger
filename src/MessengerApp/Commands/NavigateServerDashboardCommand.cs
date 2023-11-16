using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerApp.Stores;
using MessengerApp.ViewModels;
using MessengerDashboard;
using System.Diagnostics;
using MessengerDashboard.Client;

namespace MessengerApp.Commands
{
    internal class NavigateServerDashboardCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        private readonly DashboardServerViewModel _dashboardViewModel;

        //private readonly string 

        public NavigateServerDashboardCommand(NavigationStore navigationStore, DashboardServerViewModel dashboardViewModel)
        {
            _navigationStore = navigationStore;
            _dashboardViewModel = dashboardViewModel;
        }

        public override void Execute(object parameter)
        {
            _navigationStore.SubViewModel = _dashboardViewModel;
        }
    }
}
