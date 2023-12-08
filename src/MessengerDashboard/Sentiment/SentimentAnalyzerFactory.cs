/******************************************************************************
* Filename    = SentimentAnalyzerFactory.cs
*
* Author      = Aradhya Bijalwan
*
* Roll Number = 112001006
*
* Product     = MessengerApp
* 
* Project     = MessengerDashboard
*
* Description =  Provides a factory for creating and obtaining an instance
*        
*****************************************************************************/
using System;

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
