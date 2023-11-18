﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerApp.Stores;
using MessengerApp.ViewModels;
using MessengerDashboard.UI;
using System.Diagnostics;
//using MessengerDashboard.Client;
//using MessengerDashboard.UI.ViewModels;
using MessengerScreenshare.Client;

namespace MessengerApp.Commands
{
    internal class NavigateClientScreenshareCommand : CommandBase
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
        }
    }
}
