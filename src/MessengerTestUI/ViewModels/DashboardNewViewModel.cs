using System.Collections.Generic;
using System.Windows.Input;
using MessengerDashboard;

namespace MessengerTestUI.ViewModels;

public class DashboardNewViewModel : ViewModelBase
{

    public DashboardNewViewModel() 
    {
        SwitchModeCommand = new SwitchModeCommand(this) ;
        EndMeetCommand = new EndMeetCommand(this);
        RefreshCommand = new RefreshCommand(this);
    }
    private List<User> _users;
    public List<User> Users
    {
        get => _users;
        set => SetProperty(ref _users, value);
    }
    private string _summary;
    public string Summary
    {
        get => _summary;
        set => SetProperty(ref _summary, value);
    }
    private string _mode;
    public string Mode
    {
        get => _mode;
        set => SetProperty(ref _mode, value);
    }

    public ICommand SwitchModeCommand { get; set; }

    public ICommand EndMeetCommand { get; set; }
    public ICommand RefreshCommand { get; set; }
}
