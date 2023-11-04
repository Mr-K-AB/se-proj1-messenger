using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Dashboard
{
    public class SummaryDetail
    {
        public string detail;

        //     Default Constructor, necessary for serialization
        public SummaryDetail()
        {
        }

        //     Constructor to initialize the field summary with
        //     a given string.

        public SummaryDetail(string chatSummary)
        {
            detail = chatSummary;
        }
    }
}
