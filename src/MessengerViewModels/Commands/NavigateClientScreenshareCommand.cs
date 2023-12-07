/******************************************************************************
* Filename    = NavigateClientMeetCommand.cs
*
* Author      = Vinay Ingle
*
* Roll Number = 112001050
*
* Product     = Messenger 
* 
* Project     = ViewModels
*
* Description = Command to switch ViewModel to Client ScreenShare.
* *****************************************************************************/

using MessengerScreenshare.Client;
using MessengerViewModels.Stores;
using TraceLogger;

namespace MessengerViewModels.Commands
{
    public class NavigateClientScreenshareCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        private readonly ScreenshareClientViewModel _screenshareViewModel;

        //private readonly string 

        public NavigateClientScreenshareCommand(NavigationStore navigationStore, ScreenshareClientViewModel screenshareViewModel)
        {
            _navigationStore = navigationStore;
            _screenshareViewModel = screenshareViewModel;
        }

        public override void Execute(object parameter)
        {
            _navigationStore.SubViewModel = _screenshareViewModel;
            Logger.Debug("[NavigateClientScreenshareCommand] change subviewmodel to screenshareViewModel");
        }
    }
}
