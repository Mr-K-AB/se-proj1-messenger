using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Summarization;

namespace MessengerDashboard.Sentiment
{
    /// <summary>
    /// Provides a factory for creating and obtaining an instance of an <see cref="ISentimentAnalyzer"/>.
    /// </summary>
    public class SentimentAnalyzerFactory
    {
        private static readonly Lazy<SentimentAnalyzer> s_sentimentAnalyzer = new(() => new SentimentAnalyzer());


        /// <summary>
        /// Gets an instance of an object that implements the <see cref="ISentimentAnalyzer"/> interface.
        /// </summary>
        /// <returns>An instance of an object that can analyze sentiment in text messages.</returns>
        public static ISentimentAnalyzer GetSentimentAnalyzer()
        {
            return s_sentimentAnalyzer.Value;
        }
    }
}
