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
