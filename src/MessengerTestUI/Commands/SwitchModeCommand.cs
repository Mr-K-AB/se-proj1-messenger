using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerDashboard;
using MessengerTestUI.ViewModels;

namespace MessengerTestUI.Commands
{
    public class SwitchModeCommand : ICommand
    {

        private readonly ServerMeetViewModel _serverMeetViewModel;
        public SwitchModeCommand(ServerMeetViewModel viewModel)
        {
            _serverMeetViewModel = viewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (_serverMeetViewModel.Server.SessionInfo.SessionMode == SessionMode.Lab) 
            {
                _serverMeetViewModel.Server.SetExamMode();
            }
            else
            {
                _serverMeetViewModel.Server.SetLabMode();
            }
        }
    }
}
