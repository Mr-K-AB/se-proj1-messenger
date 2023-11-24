/******************************************************************************
* Filename    = DashboardFactory.cs
*
* Author      = Pratham Ravindra Nagpure
*
* Roll Number = 112001054
*
* Product     = Messenger 
* 
* Project     = Dashboard
*
* Description = A class that contains a factory for creating server and client
* instances.
* *****************************************************************************/


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

        /// <summary>
        /// Returns the singleton instance of <see cref="IServerSessionController"/>
        /// </summary>
        /// <returns></returns>
        public static IServerSessionController GetServerSessionController()
        {
            return s_serverSessionController;
        }

        /// <summary>
        /// Returns the singleton instance of <see cref="IClientSessionController"/>
        /// </summary>
        /// <returns></returns>
        public static IClientSessionController GetClientSessionController()
        {
            return s_clientSessionController;
        }
    }
}
