using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Client;
using MessengerDashboard.Server;

namespace MessengerDashboard.Telemetry
{
    public interface ITelemetry
    {
        public Analysis UpdateAnalysis(Dictionary<int, Tuple<UserInfo, List<string>>> userIdToUserInfoAndChatMap);

        public void SubscribeToServerSessionController(ServerSessionController serverSessionController);
    }
}
