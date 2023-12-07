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
* Description = Command to switch SubViewModel to Server Whiteboard.
* *****************************************************************************/

using MessengerViewModels.Stores;
using MessengerWhiteboard;
using TraceLogger;

namespace MessengerViewModels.Commands
{
    public class NavigateServerWhiteboardCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        private readonly ViewModel _whiteboardViewModel;

        //private readonly string 

        public NavigateServerWhiteboardCommand(NavigationStore navigationStore, ViewModel whiteboardViewModel)
        {
            _navigationStore = navigationStore;
            _whiteboardViewModel = whiteboardViewModel;
        }

        public override void Execute(object parameter)
        {
            _navigationStore.SubViewModel = _whiteboardViewModel;
            Logger.Debug("[NavigateWhiteboardCommand] change subviewmodel to whiteboardViewModel");
        }
    }
}
