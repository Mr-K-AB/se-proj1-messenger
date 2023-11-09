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


namespace MessengerDashboard.Client
{
    public class SessionInfo
    {
        public SessionInfo()
        {
            Users ??= new List<ClientInfo>();
            SessionMode = SessionMode.Lab;
            SessionID = new Random().Next();
        }

        public int SessionID { get; set; }
        public SessionMode SessionMode { get; set; }

        public List<ClientInfo> Users { get; set; }
    }
}
