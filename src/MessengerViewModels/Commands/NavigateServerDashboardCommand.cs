using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerViewModels.Stores;
using MessengerViewModels.ViewModels;
using MessengerDashboard.UI;
using System.Diagnostics;
using MessengerDashboard.Client;
using MessengerDashboard.UI.ViewModels;

namespace MessengerViewModels.Commands
{
    public class NavigateServerDashboardCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        private readonly DashboardInstructorViewModel _dashboardViewModel;

        //private readonly string 

        public NavigateServerDashboardCommand(NavigationStore navigationStore, DashboardInstructorViewModel dashboardViewModel)
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
