/// <credits>
/// <author>
/// <name>Shailab Chauhan</name>
/// <rollnumber>112001038</rollnumber>
/// </author>
/// </credits>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Summarization;

namespace MessengerDashboard.Client.Events
{
    public class SummaryChangedEventArgs
    {
        public TextSummary? Summary { get; set; }
    }
}
