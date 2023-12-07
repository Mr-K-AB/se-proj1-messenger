/******************************************************************************
* Filename    = NavigateStore.cs
*
* Author      = Vinay Ingle
*
* Roll Number = 112001050
*
* Product     = Messenger 
* 
* Project     = ViewModels
*
* Description = Store to enable switching the viewmodels and subviewmodels.
* *****************************************************************************/

using MessengerDashboard;
using MessengerViewModels.ViewModels;

namespace MessengerViewModels.Stores
{
    public class NavigationStore
    {
        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel?.Dispose();
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        private object _subViewModel;
        public object SubViewModel
        {
            get => _subViewModel;
            set
            {
                _subViewModel = value;
                OnSubViewModelChanged();
            }
        }

        public AuthenticationResult AuthResult { get; set; }

        public event Action CurrentViewModelChanged;

        public event Action SubViewModelChanged;

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }

        private void OnSubViewModelChanged()
        {
            SubViewModelChanged?.Invoke();
        }
    }
}
