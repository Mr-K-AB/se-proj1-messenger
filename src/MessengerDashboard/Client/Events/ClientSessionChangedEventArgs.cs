/// <credits>
/// <author>
/// <name>Shailab Chauhan</name>
/// <rollnumber>112001038</rollnumber>
/// </author>
/// </credits>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Client.Events
{
    public class ClientSessionChangedEventArgs : EventArgs
    {
        public ClientSessionChangedEventArgs(SessionInfo sessionInfo)
        {
            Session = sessionInfo;
        }

        public SessionInfo Session { get; set; }
    }
}
