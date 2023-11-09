/// <credits>
/// <author>
/// <name>Aradhya Bijalwan</name>
/// <rollnumber>112001006</rollnumber>
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
        public int SessionID { get; set; }
        public SessionMode SessionMode { get; set; }

        public List<UserInfo> Users { get; set; }

        public SessionInfo()
        {
            Users ??= new List<UserInfo>();
            SessionMode = SessionMode.Lab;
            SessionID = new Random().Next();
        }
    }
}
