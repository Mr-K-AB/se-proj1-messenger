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
    public class LocalCommand : ICommand
    {
        private readonly SessionsViewModel _sessionsViewModel;

        public LocalCommand(SessionsViewModel viewModel) 
        {
            _sessionsViewModel = viewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _sessionsViewModel.IsLocalClicked = true;
            List<EntityInfoWrapper> entities = LocalSave.ReadFromFile();
            List<SessionEntry> sessions = new();
            foreach(EntityInfoWrapper entity in entities)
            {
                sessions.Add(new SessionEntry(entity.SessionId, new ExpandCommand(_sessionsViewModel, entity)));
            }
            _sessionsViewModel.Sessions = sessions;
        }
    }
}
