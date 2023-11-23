/******************************************************************************
* Filename    = RefreshCommand.cs
*
* Author      = Satish Patidar 
*
* Roll number = 112001037
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = A Command for the refresh button in the dashboard 
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerDashboard;
using MessengerDashboard.Client;
using MessengerDashboard.Server;

namespace MessengerDashboard.UI.Commands
{
    /// <summary>
    /// Represents a command for handling the refresh button in the dashboard.
    /// </summary>
    public class RefreshCommand : ICommand
    {
        private readonly IClientSessionController _client = DashboardFactory.GetClientSessionController();

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">The parameter for the command.</param>
        /// <returns>True if the command can execute, otherwise false.</returns>
        public bool CanExecute(object? parameter)
        {
            // The command can always execute.
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">The parameter for the command.</param>
        public void Execute(object? parameter)
        {
            // Sends a refresh request to the server.
            _client.SendRefreshRequestToServer();
        }
    }
}
