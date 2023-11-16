using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Sentiment;

namespace MessengerDashboard.Client.Events
{
    public class SentimentChangedEventArgs : EventArgs
    {
        public SentimentResult Sentiment { get; }

        public SentimentChangedEventArgs(SentimentResult sentiment)
        {
            Sentiment = sentiment;
        }
    }
}
