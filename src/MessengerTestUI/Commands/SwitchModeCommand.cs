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

        private readonly DashboardServerViewModel _dashboardServerViewModel;
        public SwitchModeCommand(DashboardServerViewModel viewModel)
        {
            _dashboardServerViewModel = viewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {

        }
    }
}
