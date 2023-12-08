///******************************************************************************
// Filename    = SessionEntry.cs
//
// Author      = Satish Patidar 
//
// Roll number = 112001037
//
// Product     = Messenger 
// 
// Project     = MessengerDashboard
//
// Description = Represents a session entry in the Session
//*****************************************************************************/

using System.Windows.Input;

namespace MessengerDashboard.UI.DataModels
{
    /// <summary>
    /// Represents a session entry in the Messenger Dashboard.
    /// </summary>
    public class SessionEntry
    {
        /// <summary>
        /// Gets or sets the name of the session.
        /// </summary>
        public string SessionName { get; set; }

        /// <summary>
        /// Gets or sets the command to expand the session.
        /// </summary>
        public ICommand ExpandCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionEntry"/> class with the specified session name and expand command.
        /// </summary>
        /// <param name="sessionName">The name of the session.</param>
        /// <param name="expandCommand">The command to expand the session.</param>
        public SessionEntry(string sessionName, ICommand expandCommand)
        {
            SessionName = sessionName;
            ExpandCommand = expandCommand;
        }
    }
}

