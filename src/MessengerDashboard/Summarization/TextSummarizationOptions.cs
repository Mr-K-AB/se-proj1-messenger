using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Summarization
{
    public class TextSummarizationOptions
    {
        public int MaxSummarySentences { get; set; } = 15;

        public int MaxSummaryPercentage { get; set; } = 10;
    }
}
