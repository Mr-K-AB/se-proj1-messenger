/******************************************************************************
* Filename    = DeleteAllCommand.cs
* 
* Author      = Satish Patidar 
*
* Roll number = 112001037
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = A Command for delete all sessions
*****************************************************************************/

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerCloud;
using MessengerDashboard.UI.ViewModels;

namespace MessengerDashboard.UI.Commands
{
    /// <summary>
    /// Represents a command for deleting all sessions in the Messenger Dashboard.
    /// </summary>
    public class DeleteAllCommand : ICommand
    {
        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        private readonly SessionsViewModel _sessionsViewModel;

        private readonly RestClient _restClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteAllCommand"/> class.
        /// </summary>
        /// <param name="restClient">The RestClient used for making API calls.</param>
        /// <param name="sessionsViewModel">The view model containing session information.</param>
        public DeleteAllCommand(RestClient restClient, SessionsViewModel sessionsViewModel)
        {
            _sessionsViewModel = sessionsViewModel ?? throw new ArgumentNullException(nameof(sessionsViewModel));
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        }

        /// <inheritdoc/>
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <inheritdoc/>
        public void Execute(object? parameter)
        {
            if (_sessionsViewModel.IsLocalClicked == true)
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appDataFolder = Path.Combine(localAppData, "Messenger");
                string path = Path.Combine(appDataFolder, "sessionInfo.txt");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            else if (_sessionsViewModel.IsLocalClicked == false)
            {
                Thread something = new(() =>
                {
                    try
                    {
                        Task task = _restClient.DeleteEntitiesAsync();
                        task.Wait();
                        _sessionsViewModel.Sessions = new();
                    }
                    catch (Exception e) { /* Log or handle the exception */ }
                })
                { IsBackground = true };
                something.Start();
                something.Join();
            }
        }
    }
}
