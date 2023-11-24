/******************************************************************************
* Filename    = MainViewModel.cs
*
* Author      = Vinay Ingle
*
* Roll Number = 112001050
*
* Product     = Messenger 
* 
* Project     = ViewModels
*
* Description = The ViewModel in which the other ViewModels will be changes.
* *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerViewModels.Stores;

namespace MessengerViewModels.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

        public MainViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            navigationStore.CurrentViewModelChanged += NavigationStore_CurrentViewModelChanged;
        }

        private void NavigationStore_CurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
