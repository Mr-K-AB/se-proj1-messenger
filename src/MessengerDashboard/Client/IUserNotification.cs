﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Dashboard;

namespace MessengerDashboard.Client
{
    public interface IUserNotification
    {
        void OnUserSessionChange(SessionInfo session);
    }
}