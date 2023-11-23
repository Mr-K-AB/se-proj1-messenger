using MessengerViewModels.Stores;
using MessengerViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MessengerViewModels.ViewModels
{
    public class MeetViewModel : ViewModelBase
    {
        public string VMString => "Meet";
        public ICommand NavigateHomeCommand { get; }

        public MeetViewModel(NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateHomeCommand(navigationStore);
        }
    }
}
