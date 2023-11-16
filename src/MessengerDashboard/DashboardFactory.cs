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
        private static readonly ServerSessionController s_serverSessionController = new();

        private static readonly ClientSessionController s_clientSessionController = new();

        public static IServerSessionController GetServerSessionController()
        {
            return s_serverSessionController;
        }

        public static IClientSessionController GetClientSessionController()
        {
            return s_clientSessionController;
        }
    }
}
