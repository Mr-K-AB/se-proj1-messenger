using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MessengerDashboard.UI.DataModels
{
    public class TimeStampChatCountEntry
    {
        public DateTime TimeStamp { get; set; }

        public int ChatCount { get; set; }

        public TimeStampChatCountEntry(DateTime timeStamp, int chatCount)
        {
            TimeStamp = timeStamp;
            ChatCount = chatCount;
        }
    }
}
