using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SentimentAnalyzer;

namespace MessengerDashboard.Sentiment
{
    public class SentimentAnalyzer : ISentimentAnalyzer
    {
        public SentimentResult AnalyzeSentiment(string[] chats) 
        {
            int positiveChatCount = 0;
            int negativeChatCount = 0;
            StringBuilder sb = new();
            for (int i = 0; i < chats.Length; i++)
            {
                sb.Append(chats[i]);
                if (Sentiments.Predict(chats[i]).Prediction)
                {
                    positiveChatCount++;
                }
                else
                {
                    negativeChatCount++;
                }
            }
            bool isOverallSentimentPositive = Sentiments.Predict(sb.ToString()).Prediction;
            return new SentimentResult()
            {
                PositiveChatCount = positiveChatCount,
                NegativeChatCount = negativeChatCount,
                IsOverallSentimentPositive = isOverallSentimentPositive
            };
        }
    }
}
