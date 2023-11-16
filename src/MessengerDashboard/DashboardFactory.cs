using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Client;
using MessengerDashboard.Server;

namespace MessengerDashboard
{
    public static class DashboardFactory
    {
        private static readonly Lazy<ServerSessionController> s_serverSessionController = new(() => new ServerSessionController());

        private static readonly Lazy<ClientSessionController> s_clientSessionController = new(() => new ClientSessionController());

        public static IServerSessionController GetServerSessionController()
        {
            return s_serverSessionController.Value;
        }

        public static IClientSessionController GetClientSessionController()
        {
            return s_clientSessionController.Value;
        }
    }
}
