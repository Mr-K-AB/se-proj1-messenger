using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerCloud;
using MessengerDashboard.UI.ViewModels;

namespace MessengerDashboard.UI.Commands
{
    public class DeleteAllCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly SessionsViewModel _sessionsViewModel;

        private readonly RestClient _restClient;

        public DeleteAllCommand(RestClient restClient, SessionsViewModel sessionsViewModel)
        {
            _sessionsViewModel = sessionsViewModel;
            _restClient = restClient;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

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
                    catch (Exception e) { }
                })
                { IsBackground = true };
                something.Start();
                something.Join();
            }
        }
    }
}
