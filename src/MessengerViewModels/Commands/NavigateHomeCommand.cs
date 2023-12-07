/******************************************************************************
* Filename    = NavigateHomeCommand.cs
*
* Author      = Vinay Ingle
*
* Roll Number = 112001050
*
* Product     = Messenger 
* 
* Project     = ViewModels
*
* Description = Command to switch Current ViewModel to HomeViewModel.
* *****************************************************************************/

using MessengerViewModels.Stores;
using MessengerViewModels.ViewModels;

namespace MessengerViewModels.Commands
{
    public class NavigateHomeCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        public NavigateHomeCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }

        public override void Execute(object parameter)
        {
            _navigationStore.CurrentViewModel = new HomeViewModel(_navigationStore);
        }
    }
}
