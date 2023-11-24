/******************************************************************************
* Filename    = NavigateClientDashboardCommand.cs
*
* Author      = Vinay Ingle
*
* Roll Number = 112001050
*
* Product     = Messenger 
* 
* Project     = ViewModels
*
* Description = Command to navigate to Client Dashboard Instance and change View.
* *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerViewModels.Stores;
using MessengerViewModels.ViewModels;
using MessengerDashboard.UI;
using MessengerDashboard;
using System.Diagnostics;
using MessengerDashboard.UI.ViewModels;

namespace MessengerViewModels.Commands
{
    public class NavigateClientDashboardCommand : CommandBase
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
