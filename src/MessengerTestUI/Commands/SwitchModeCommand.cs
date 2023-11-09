using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MessengerTestUI.Commands
{
    public class SwitchModeCommand : ICommand
    {
        public SwitchModeCommand(object viewModel, Type type) {  }

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
