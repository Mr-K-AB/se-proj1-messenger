﻿/******************************************************************************
* Filename    = IUserNotification.cs
*
* Author      = Shailab Chauhan 
*
* Roll number = 112001038
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard

*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Client
{
    /// <summary>
    /// Interface representing a user notification for session changes in the Messenger Dashboard project.
    /// </summary>
    public interface IUserNotification
    {
        /// <summary>
        /// Notifies about a change in the user session.
        /// </summary>
        /// <param name="session">Information about the current session.</param>
        void OnUserSessionChange(SessionInfo session);
    }
}
