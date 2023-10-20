using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Represents the result of a sentiment analysis for a collection of chat messages.
/// </summary>
namespace MessengerDashboard.Sentiment
{
    public class SentimentResult
    {
        /// <summary>
        /// Gets or sets the count of chat messages with positive sentiment.
        /// </summary>
        public int PositiveChatCount { get; set; }

        /// <summary>
        /// Gets or sets the count of chat messages with negative sentiment.
        /// </summary>
        public int NegativeChatCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the overall sentiment of the chat messages is positive.
        /// </summary>
        public bool IsOverallSentimentPositive { get; set; }
    }
}
