using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerTestUI.ViewModels;

namespace MessengerTestUI.Commands
{
    public class EndMeetCommand : ICommand
    {
        private readonly ClientMeetViewModel? _clientMeetViewModel;
        private readonly ServerMeetViewModel? _serverMeetViewModel;
        public EndMeetCommand(object viewModel) 
        {
            if (viewModel is ClientMeetViewModel model)
            {
                _clientMeetViewModel = model;
            }
            else
            {
                _serverMeetViewModel = (ServerMeetViewModel) viewModel;
            }
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _clientMeetViewModel?.Client.SendExitSessionRequestToServer(null);
            _serverMeetViewModel?.Server.EndSession();
        }
    }
}
