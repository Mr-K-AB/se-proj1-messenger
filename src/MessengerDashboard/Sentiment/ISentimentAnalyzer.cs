using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Sentiment
{
    public interface ISentimentAnalyzer
    {
        SentimentResult AnalyzeSentiment(string[] chats);
    }
}
