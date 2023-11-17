using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerApp.Stores;
using MessengerApp.ViewModels;
using MessengerDashboard.UI;
using MessengerDashboard;
using System.Diagnostics;
using MessengerDashboard.UI.ViewModels;

namespace MessengerApp.Commands
{
    internal class NavigateClientDashboardCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly DashboardMemberViewModel _dashboardMemberViewModel;
        //private readonly DashboardClientViewModel _dashboardClientViewModel;

        //private readonly string 

        public NavigateClientDashboardCommand(NavigationStore navigationStore, DashboardMemberViewModel dashboardMemberViewModel)
        {
            _navigationStore = navigationStore;
            _dashboardMemberViewModel = dashboardMemberViewModel;
        }

        public override void Execute(object parameter)
        {
            _navigationStore.SubViewModel = _dashboardMemberViewModel;
        }
    }
}
