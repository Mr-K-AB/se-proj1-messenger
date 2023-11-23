using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MessengerDashboard.UI.DataModels
{
    public class TimeStampUserCountEntry
    {
        public DateTime TimeStamp { get; set; }

        public int UserCount { get; set; }

        public TimeStampUserCountEntry(DateTime timeStamp, int chatCount)
        {
            TimeStamp = timeStamp;
            UserCount = chatCount;
        }
    }
}
