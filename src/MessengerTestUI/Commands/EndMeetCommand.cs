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
        private readonly DashboardClientViewModel? _dashboardClientViewModel;
        private readonly DashboardServerViewModel? _dashboardServerViewModel;
        public EndMeetCommand(object viewModel) 
        {
            if (viewModel is DashboardClientViewModel model)
            {
                _dashboardClientViewModel = model;
            }
            else
            {
                _dashboardServerViewModel = (DashboardServerViewModel) viewModel;
            }
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _dashboardClientViewModel?.Client.RequestServerToRemoveClient(null);
            _dashboardServerViewModel?.Server.EndSession();
        }
    }
}
