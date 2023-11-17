﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Client.Events
{
    public class SessionModeChangedEventArgs : EventArgs
    {
        public SessionMode SessionMode { get; }

        public SessionModeChangedEventArgs(SessionMode sessionMode)
        {
            SessionMode = sessionMode;
        }
    }
}
