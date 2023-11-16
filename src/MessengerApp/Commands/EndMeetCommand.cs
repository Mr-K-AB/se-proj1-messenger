using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.UI.ViewModels;
using System.Windows.Input;
using MessengerApp.ViewModels;

namespace MessengerApp.Commands
{
    public class EndMeetCommand : ICommand
    {
        private readonly DashboardMemberViewModel? _dashboardClientViewModel;
        private readonly DashboardInstructorViewModel? _dashboardServerViewModel;
        
        public EndMeetCommand(object viewModel) 
        {
            if (viewModel is DashboardMemberViewModel model)
            {
                _dashboardClientViewModel = model;
            }
            else
            {
                _dashboardServerViewModel = (DashboardInstructorViewModel) viewModel;
            }
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            //_dashboardClientViewModel?.Client.RequestServerToRemoveClient(null);
            //_dashboardServerViewModel?.Server.EndSession();
        }
    }
}
