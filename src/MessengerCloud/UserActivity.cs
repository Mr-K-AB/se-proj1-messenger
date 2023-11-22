using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerCloud
{
    public class UserActivity
    {
        public int UserChatCount { get; set; }

        public string? UserName { get; set; }

        public string? UserEmail { get; set; }

        public DateTime EntryTime { get; set; }

        public DateTime ExitTime { get; set; } = DateTime.MinValue;
    }
}
