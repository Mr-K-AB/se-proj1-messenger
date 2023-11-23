/******************************************************************************
* Filename    = SessionModeChangedEventArgs.cs
*
* Author      = Shailab Chauhan 
*
* Roll number = 112001038
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = Provides data for the event when the session mode changes.
*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Client.Events
{
    /// <summary>
    /// Provides data for the event when the session mode changes.
    /// </summary>
    public class SessionModeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the new session mode.
        /// </summary>
        public SessionMode SessionMode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionModeChangedEventArgs"/> class with the specified session mode.
        /// </summary>
        /// <param name="sessionMode">The new session mode.</param>
        public SessionModeChangedEventArgs(SessionMode sessionMode)
        {
            SessionMode = sessionMode;
        }
    }
}
