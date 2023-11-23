/******************************************************************************
* Filename    = EndMeetCommand.cs
*
* Author      = Satish Patidar 
*
* Roll number = 112001037
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = A Command for ending the meeting in the dashboard
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerDashboard.Client;

namespace MessengerDashboard.UI.Commands
{
    /// <summary>
    /// Represents a command for ending a meeting in the dashboard.
    /// </summary>
    public class EndMeetCommand : ICommand
    {
        /// <summary>
        /// The client session controller for communication with the server.
        /// </summary>
        private readonly IClientSessionController _client = DashboardFactory.GetClientSessionController();

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <inheritdoc/>
        public bool CanExecute(object? parameter)
        {
            // The command can always be executed.
            return true;
        }

        /// <inheritdoc/>
        public void Execute(object? parameter)
        {
            // Sends a request to the server to exit the session.
            _client.SendExitSessionRequestToServer();
        }
    }
}
