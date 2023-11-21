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
using MessengerDashboard.Sentiment;

namespace MessengerDashboard.Client
{
    public class ClientPayload
    {
        public ClientPayload(Operation eventName, UserInfo? userInfo)
        {
            Operation = eventName;
            UserInfo = userInfo;
        }

        public ClientPayload()
        {

        }

        public Operation Operation { get; set; }

        public UserInfo UserInfo { get; set; } = new ();
    }
}
