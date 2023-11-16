﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerDashboard.Client;

namespace MessengerDashboard.UI.Commands
{
    public class EndMeetCommand : ICommand
    {

        private readonly IClientSessionController _client = DashboardFactory.GetClientSessionController();

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _client.SendExitSessionRequestToServer();
        }
    }
}