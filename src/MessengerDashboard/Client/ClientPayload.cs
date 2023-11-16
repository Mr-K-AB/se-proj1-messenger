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
        public ClientPayload(Operation eventName, string ipAddress, int port, UserInfo userInfo)
        {
            Operation = eventName;
            IpAddress = ipAddress;
            Port = port;
            UserInfo = userInfo;
        }

        public ClientPayload()
        {

        }

        public string? IpAddress { get; set; } = string.Empty;
        
        public Operation Operation { get; set; }

        public int Port { get; set; } = -1;

        public UserInfo? UserInfo { get; set; }
    }
}
