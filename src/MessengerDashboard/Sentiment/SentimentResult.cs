using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Sentiment
{
    public class SentimentResult
    {
        public int PositiveChatCount { get; set; }

        public int NegativeChatCount { get; set; }

        public bool IsOverallSentimentPositive { get; set; }
    }
}
