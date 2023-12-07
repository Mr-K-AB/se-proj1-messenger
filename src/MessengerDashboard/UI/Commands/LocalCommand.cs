/******************************************************************************
* Filename    = LocalCommand.cs
*
* Author      = Satish Patidar 
*
* Roll number = 112001037
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = A Command for getting local database in session 
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerCloud;
using MessengerDashboard.UI.DataModels;
using MessengerDashboard.UI.ViewModels;

namespace MessengerDashboard.UI.Commands
{
    /// <summary>
    /// Represents a command for retrieving the local database in a session.
    /// </summary>
    public class LocalCommand : ICommand
    {
        private readonly SessionsViewModel _sessionsViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalCommand"/> class.
        /// </summary>
        public LocalCommand()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalCommand"/> class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model associated with this command.</param>
        public LocalCommand(SessionsViewModel viewModel)
        {
            _sessionsViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

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
            // Set the flag indicating that the local button is clicked.
            _sessionsViewModel.IsLocalClicked = true;
            _sessionsViewModel.SelectedSession = "Selected: Local";
            _sessionsViewModel.PositiveChatCount = 0; 
            _sessionsViewModel.NegativeChatCount = 0;
            _sessionsViewModel.NeutralChatCount = 0;
            _sessionsViewModel.OverallSentiment = "";
            _sessionsViewModel.TotalUserCount = 0;
            _sessionsViewModel.SessionSummary = "";
            _sessionsViewModel.TimeStampUserCountEntries = new();
            _sessionsViewModel.UserActivities = new();
            _sessionsViewModel.Sessions = new();

            // Read entities from the local file.
            List<EntityInfoWrapper> entities = LocalSave.ReadFromFile();

            // Create session entries from the entities.
            List<SessionEntry> sessions = new();
            foreach (EntityInfoWrapper entity in entities)
            {
                sessions.Add(new SessionEntry(entity.SessionId, new ExpandCommand(_sessionsViewModel, entity)));
            }

            // Set the view model sessions with the created sessions.
            _sessionsViewModel.Sessions = sessions;
        }
    }
}
