/******************************************************************************
* Filename    = CloudCommand.cs
*
* Author      = Satish Patidar 
*
* Roll number = 112001037
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = A Command for getting cloud database in session
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerCloud;
using MessengerDashboard.UI.DataModels;
using MessengerDashboard.UI.ViewModels;

namespace MessengerDashboard.UI.Commands
{
    /// <summary>
    /// Represents a command for retrieving data from the cloud database and updating the session view model.
    /// </summary>
    public class CloudCommand : ICommand
    {
        private readonly RestClient _restClient;
        private readonly SessionsViewModel _sessionsViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudCommand"/> class.
        /// </summary>
        /// <param name="restClient">The RestClient instance for making requests to the cloud.</param>
        /// <param name="sessionsViewModel">The view model for managing sessions.</param>
        public CloudCommand(RestClient restClient, SessionsViewModel sessionsViewModel)
        {
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
            _sessionsViewModel = sessionsViewModel ?? throw new ArgumentNullException(nameof(sessionsViewModel));
        }

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <inheritdoc/>
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <inheritdoc/>
        public void Execute(object? parameter)
        {
            // Set the local clicked property to false
            _sessionsViewModel.IsLocalClicked = false;
            _sessionsViewModel.SelectedSession = "Selected: Cloud";
            _sessionsViewModel.PositiveChatCount = 0; 
            _sessionsViewModel.NegativeChatCount = 0;
            _sessionsViewModel.NeutralChatCount = 0;
            _sessionsViewModel.OverallSentiment = "";
            _sessionsViewModel.TotalUserCount = 0;
            _sessionsViewModel.SessionSummary = "";
            _sessionsViewModel.TimeStampUserCountEntries = new();
            _sessionsViewModel.UserActivities = new();
            _sessionsViewModel.Sessions = new();

            // Start a new thread to perform the asynchronous cloud data retrieval
            Thread cloudDataRetrievalThread = new(() =>
            {
                try
                {
                    // Wait for the asynchronous task to complete
                    Task<IReadOnlyList<Entity>?> task = _restClient.GetEntitiesAsync();
                    task.Wait();
                    IReadOnlyList<Entity> results = task.Result;

                    // Process the results and update the session view model
                    List<EntityInfoWrapper> entities = new();
                    foreach (Entity entity in results)
                    {
                        entities.Add(new(entity.Sentences, entity.PositiveChatCount, entity.NegativeChatCount, entity.NeutralChatCount, entity.OverallSentiment, entity.SessionId, entity.Analysis));
                    }

                    List<SessionEntry> sessions = new();
                    foreach (EntityInfoWrapper entity in entities)
                    {
                        sessions.Add(new SessionEntry(entity.SessionId, new ExpandCommand(_sessionsViewModel, entity)));
                    }

                    _sessionsViewModel.Sessions = sessions;
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the execution
                    // (Note: In a production environment, consider logging the exception details)
                }
            })
            { IsBackground = true };

            // Start and wait for the thread to complete
            cloudDataRetrievalThread.Start();
            cloudDataRetrievalThread.Join();
        }
    }
}
