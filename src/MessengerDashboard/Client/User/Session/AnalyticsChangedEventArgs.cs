﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Dashboard;

namespace MessengerDashboard.Client.User.Session
{
    public class AnalyticsChangedEventArgs
    {
        public SessionAnalytics? SessionAnalytics { get; set; }
    }
}
