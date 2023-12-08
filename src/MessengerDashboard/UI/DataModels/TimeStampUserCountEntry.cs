/******************************************************************************
* Filename    = TimeStampUserCountEntry.cs
*
* Author      = Satish Patidar 
*
* Roll number = 112001037
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = Represents an entry containing a timestamp and the corresponding 
*               user count for the Session.
*****************************************************************************/

using System;

namespace MessengerDashboard.UI.DataModels
{
    /// <summary>
    /// Represents an entry containing a timestamp and the corresponding user count 
    /// for the MessengerDashboard.
    /// </summary>
    public class TimeStampUserCountEntry
    {
        /// <summary>
        /// Gets or sets the timestamp of the entry.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the user count associated with the entry.
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeStampUserCountEntry"/> class.
        /// </summary>
        /// <param name="timeStamp">The timestamp of the entry.</param>
        /// <param name="userCount">The user count associated with the entry.</param>
        public TimeStampUserCountEntry(DateTime timeStamp, int userCount)
        {
            TimeStamp = timeStamp;
            UserCount = userCount;
        }
    }
}
