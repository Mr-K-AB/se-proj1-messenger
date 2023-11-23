/******************************************************************************
* Filename    = SentimentAnalyzer.cs
*
* Author      = Aradhya Bijalwan
*
* Product     = MessengerApp
* 
* Project     = MessengerDashboard
*
* Description = providing sentiment analysis  for an array of chat messages using the SentimentAnalyzer library.
*****************************************************************************/
using System.Text;
using VaderSharp2;

namespace MessengerDashboard.Sentiment
{
    /// <summary>
    /// Represents a sentiment analyzer.
    /// </summary>
    /// <remarks>
    /// Implements <see cref="ISentimentAnalyzer"/>
    /// </remarks>
    public class SentimentAnalyzer : ISentimentAnalyzer
     
    { 
        readonly SentimentIntensityAnalyzer _sentimentAnalyzer = new();
        /// <summary>
        /// Analyzes the sentiment of an array of chat messages.
        /// </summary>
        /// <Param name="chats">Array of chat messages to analyze.</Param>
        /// <returns>A <see cref="SentimentResult"/> object containing the analysis results.</returns>

        public SentimentResult AnalyzeSentiment(string[] chats) 
        {   
            int positiveChatCount = 0;
            int negativeChatCount = 0;
            int neutralChatCount = 0;
            double score;
            StringBuilder sb = new();
            for (int i = 0; i < chats.Length; i++)
            {
                score = _sentimentAnalyzer.PolarityScores(chats[i]).Compound;
                string sentiment = SentimentFromScore(score);
                if (sentiment == "Positive")
                {
                    positiveChatCount++;
                }
                else if (sentiment == "Negative")
                {
                    negativeChatCount++;
                }
                else
                {
                    neutralChatCount++;
                }
                sb.Append(chats[i]);
            }
            score = _sentimentAnalyzer.PolarityScores(sb.ToString()).Compound;
            return new SentimentResult()
            {
                PositiveChatCount = positiveChatCount,
                NegativeChatCount = negativeChatCount,
                NeutralChatCount = neutralChatCount,
                OverallSentiment = SentimentFromScore(score) 
            };
        }
        /// <summary>
        /// return positive ,negative ,neutral for a string value
        /// </summary>
        /// <Param name="scoreq">score of session</Param>
        /// <returns>A <see cref="SentimentResult"/> object containing the analysis results.</returns>

        private static string SentimentFromScore(double score)
        {
            if (score > 0.05)
            {
                return "Positive";
            }
            if (score < -0.05)
            {
                return "Negative";
            }
            return "Neutral";
        }
    }
}
