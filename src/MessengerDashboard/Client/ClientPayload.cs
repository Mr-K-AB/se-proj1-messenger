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
        public ClientPayload(Operation eventName, string ipAddress, int port, UserInfo? userInfo, string clientSessionControllerId)
        {
            Operation = eventName;
            IpAddress = ipAddress;
            Port = port;
            UserInfo = userInfo;
            ClientSessionControllerId = clientSessionControllerId;
        }

        public ClientPayload()
        {

        }

        public string? IpAddress { get; set; }
        
        public Operation Operation { get; set; }

        public int Port { get; set; } = -1;

        public UserInfo? UserInfo { get; set; }

        public string ClientSessionControllerId { get; set; }
    }
}
