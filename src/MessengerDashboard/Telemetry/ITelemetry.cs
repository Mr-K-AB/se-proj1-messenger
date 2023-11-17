using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Client;

namespace MessengerDashboard.Telemetry
{
    public interface ITelemetry
    {
        public Analysis UpdateAnalysis(Dictionary<int, Tuple<UserInfo, List<string>>> userIdToUserInfoAndChatMap);
    }
}
