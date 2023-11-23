/******************************************************************************
* Filename    = UserActivityEntry.cs
*
* Author      = Satish Patidar 
*
* Roll number = 112001037
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description =  Represents the details of user
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.UI.DataModels
{
    /// <summary>
    /// Represents an entry for user activity in the Messenger Dashboard.
    /// </summary>
    public class UserActivityEntry
    {
        /// <summary>
        /// Gets the unique identifier of the user.
        /// </summary>
        public int UserId { get; }

        /// <summary>
        /// Gets the count of user chats.
        /// </summary>
        public int UserChatCount { get; }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        public string? UserName { get; }

        /// <summary>
        /// Gets the email of the user.
        /// </summary>
        public string? UserEmail { get; }

        /// <summary>
        /// Gets the entry time of the user activity.
        /// </summary>
        public DateTime EntryTime { get; }

        /// <summary>
        /// Gets the exit time of the user activity.
        /// </summary>
        public DateTime ExitTime { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserActivityEntry"/> class.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="userChatCount">The count of user chats.</param>
        /// <param name="userName">The name of the user.</param>
        /// <param name="userEmail">The email of the user.</param>
        /// <param name="entryTime">The entry time of the user activity.</param>
        /// <param name="exitTime">The exit time of the user activity.</param>
        public UserActivityEntry(int userId, int userChatCount, string? userName, string? userEmail, DateTime entryTime, DateTime exitTime)
        {
            UserId = userId;
            UserChatCount = userChatCount;
            UserName = userName;
            UserEmail = userEmail;
            EntryTime = entryTime;
            ExitTime = exitTime;
        }
    }
}
