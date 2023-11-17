using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MessengerDashboard.UI.DataModels
{
    public class SessionEntry
    {
        public string SessionName { get; set; }

        public ICommand ExpandCommand { get; set; }

        public SessionEntry(string sessionName, ICommand expandCommand)
        {
            SessionName = sessionName;
            ExpandCommand = expandCommand;
        }
    }
}
