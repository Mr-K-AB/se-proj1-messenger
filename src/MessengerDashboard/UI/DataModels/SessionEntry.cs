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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MessengerDashboard.UI.DataModels
{
    /// <summary>
    /// Represents a session entry in the Messenger Dashboard.
    /// </summary>
    public class SessionEntry
    {
        private int _v1;
        private int _v2;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionEntry"/> class with the specified values.
        /// </summary>
        /// <param name="v1">The value of the first parameter.</param>
        /// <param name="v2">The value of the second parameter.</param>
        public SessionEntry(int v1, int v2)
        {
            _v1 = v1;
            _v2 = v2;
        }
    }
}

