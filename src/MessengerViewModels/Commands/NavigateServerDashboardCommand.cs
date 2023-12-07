/******************************************************************************
* Filename    = NavigateServerDashboardCommand.cs
*
* Author      = Vinay Ingle
*
* Roll Number = 112001050
*
* Product     = Messenger 
* 
* Project     = ViewModels
*
* Description = Command to switch the SubViewModel to Server Dashboard.
* *****************************************************************************/

using MessengerDashboard.UI.ViewModels;
using MessengerViewModels.Stores;
using TraceLogger;

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
            Logger.Debug("[NavigateServerDashboardCommand] change subviewmodel to dashboardViewModel");
        }
    }
}
