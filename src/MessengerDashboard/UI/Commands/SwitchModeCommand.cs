/******************************************************************************
 * Filename    = SwitchModeCommand.cs
 *
 * Author      = Satish Patidar 
 *
 * Roll number = 112001037
 *
 * Product     = Messenger 
 * 
 * Project     = MessengerDashboard
 *
 * Description = A Command for changing the mode from lab to exam and vice versa in the dashboard
 *****************************************************************************/

using System;
using System.Windows.Input;
using MessengerDashboard.Client;

namespace MessengerDashboard.UI.Commands
{
    /// <summary>
    /// Command for switching the mode between lab and exam in the Messenger Dashboard.
    /// </summary>
    public class SwitchModeCommand : ICommand
    {
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
            // Switch the mode based on the current session mode.
            if (_client.SessionInfo.SessionMode == SessionMode.Lab)
            {
                // If the current mode is lab, send a request to switch to exam mode.
                _client.SendExamModeRequestToServer();
            }
            else
            {
                // If the current mode is exam, send a request to switch to lab mode.
                _client.SendLabModeRequestToServer();
            }
        }
    }
}
