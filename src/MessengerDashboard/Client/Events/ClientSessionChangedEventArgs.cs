/******************************************************************************
* Filename    = ClientSessionChangedEventArgs.cs
*
* Author      = Shailab Chauhan 
*
* Roll number = 112001038
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = Provides data for the event when the client session changes
*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Client.Events
{
    /// <summary>
    /// Provides data for the event when the client session changes.
    /// </summary>
    public class ClientSessionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSessionChangedEventArgs"/> class with the specified session information.
        /// </summary>
        /// <param name="sessionInfo">The information about the current session.</param>
        public ClientSessionChangedEventArgs(SessionInfo sessionInfo)
        {
            Session = sessionInfo;
        }

        /// <summary>
        /// Gets or sets the information about the current session.
        /// </summary>
        public SessionInfo Session { get; set; }
    }
}
