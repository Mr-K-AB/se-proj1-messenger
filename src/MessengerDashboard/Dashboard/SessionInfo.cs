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


namespace MessengerDashboard.Dashboard
{
    public class SessionInfo
    {
        public int sessionId;
        public string sessionType;
        public List<UserInfo> users;
        public SessionInfo() 
        {
            users ??= new List<UserInfo>();
            sessionType = "LabMode";
            sessionId = new Random().Next();
        }
    }
}
