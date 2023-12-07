/******************************************************************************
* Filename    = NavigateServerScreenshare.cs
*
* Author      = Vinay Ingle
*
* Roll Number = 112001050
*
* Product     = Messenger 
* 
* Project     = ViewModels
*
* Description = Command to switch SubViewModel to ScreenShare.
* *****************************************************************************/

using MessengerScreenshare.Server;
using MessengerViewModels.Stores;
using TraceLogger;

namespace MessengerViewModels.Commands
{
    public class NavigateServerScreenshareCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        private readonly ScreenshareServerViewModel _screenshareViewModel;

        //private readonly string 

        public NavigateServerScreenshareCommand(NavigationStore navigationStore, ScreenshareServerViewModel screenshareViewModel)
        {
            _navigationStore = navigationStore;
            _screenshareViewModel = screenshareViewModel;
        }

        public override void Execute(object parameter)
        {
            _navigationStore.SubViewModel = _screenshareViewModel;
            Logger.Debug("[NavigateServerScreenshareCommand] change subviewmodel to screenshareViewModel");
        }
    }
}
