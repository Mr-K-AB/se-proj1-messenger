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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerViewModels.Stores;
using MessengerViewModels.ViewModels;
using MessengerDashboard.UI;
using System.Diagnostics;
//using MessengerDashboard.Client;
//using MessengerDashboard.UI.ViewModels;
using MessengerScreenshare.Server;

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
