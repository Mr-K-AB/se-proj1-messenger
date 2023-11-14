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
using MessengerNetworking.NotificationHandler;

namespace MessengerDashboard.Client
{
    public interface IClientSessionController : INotificationHandler
    {
        bool ConnectToServer(
            string serverIpAddress,
            int serverPort,
            int? timeoutInMilliseconds,
            string clientUsername,
            string clientEmail,
            string clientPhotoUrl
        );
    }
}
