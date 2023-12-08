/******************************************************************************
* Filename    = SessionInfo.cs
*
* Author      = Shailab Chauhan 
*
* Roll number = 112001038
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = Represents information about the current session .
*****************************************************************************/

using System;
using System.Collections.Generic;


namespace MessengerDashboard.Client
{
    /// <summary>
    /// Represents information about the current session.
    /// </summary>
    public class SessionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionInfo"/> class.
        /// </summary>
        public SessionInfo()
        {
            Users ??= new List<UserInfo>();
            SessionMode = SessionMode.Lab;
            SessionID = new Random().Next();
        }

        /// <summary>
        /// Gets or sets the unique identifier for the session.
        /// </summary>
        public int SessionID { get; set; }

        /// <summary>
        /// Gets or sets the mode of the session (e.g., Lab, Exam).
        /// </summary>
        public SessionMode SessionMode { get; set; }

        /// <summary>
        /// Gets or sets the list of users participating in the session.
        /// </summary>
        public List<UserInfo> Users { get; set; }
    }
}
