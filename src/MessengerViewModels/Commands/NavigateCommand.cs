using MessengerViewModels.Services;
using MessengerViewModels.Stores;
using MessengerViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerViewModels.Commands
{
    public class NavigateCommand : CommandBase
    {
        private readonly INavigationService _navigationService;

        public NavigateCommand(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void Execute(object parameter)
        {
            _navigationService.Navigate();
        }
    }
}
