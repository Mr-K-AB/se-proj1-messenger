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

using MessengerDashboard.UI.ViewModels;
using MessengerViewModels.Stores;

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
            TraceLogger.Logger.Debug("[NavigateClientMeetCommand] In client, on dashboard click, SubViewModel changed to DashboardMemberViewModel");
        }
    }
}
