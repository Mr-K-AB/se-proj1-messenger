using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Summarization;

namespace MessengerDashboard.Client
{
    public class SummaryChangedEventArgs
    {
        public TextSummary? Summary { get; set; }
    }
}
