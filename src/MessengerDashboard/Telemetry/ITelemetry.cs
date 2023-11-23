/******************************************************************************
* Filename    = ITelemetry.cs
*
* Author      = Aradhya Bijalwan
*
* Product     = MessengerApp
* 
* Project     = MessengerDashboard
*
* Description = interface for telemetry
*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Client;
using MessengerDashboard.Server;

namespace MessengerDashboard.Telemetry
{
    /// <summary>
    /// Provides an interface for telemetry
    /// </summary>
    public interface ITelemetry
    {
        /// <summary>
        /// Updates session analysis based on the provided mapping of user IDs to user information and chat data.
        /// </summary>
        /// <Param name="userIdToUserInfoAndChatMap">Mapping of user IDs to user information and chat data.</Param>
        /// <returns>An instance of the Analysis class representing the updated session analysis.</returns>
        public Analysis UpdateAnalysis(Dictionary<int, Tuple<UserInfo, List<string>>> userIdToUserInfoAndChatMap);
      
        /// <summary>
        /// Subscribes to the specified server session controller for telemetry purposes.
        /// </summary>
        /// <Param name="serverSessionController">Server session controller to subscribe to session side</Param>
        public void SubscribeToServerSessionController(ServerSessionController serverSessionController);
    }
}
