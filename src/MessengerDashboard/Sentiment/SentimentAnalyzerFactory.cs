using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Summarization;

namespace MessengerDashboard.Sentiment
{
    public class SentimentAnalyzerFactory
    {
        private static readonly Lazy<SentimentAnalyzer> s_sentimentAnalyzer = new(() => new SentimentAnalyzer());

        public static ISentimentAnalyzer GetSentimentAnalyzer()
        {
            return s_sentimentAnalyzer.Value;
        }
    }
}
