using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.UI.DataModels
{
    public class UserActivityEntry
    {
        public int UserId { get; }

        public int UserChatCount { get; }

        public string? UserName { get; }

        public string? UserEmail { get; }

        public DateTime EntryTime { get; }

        public DateTime ExitTime { get; }

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
