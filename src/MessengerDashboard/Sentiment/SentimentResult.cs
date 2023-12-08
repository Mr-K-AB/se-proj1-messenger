/******************************************************************************
* Filename    = SentimentResult.cs
*
* Author      = Aradhya Bijalwan
*
* Roll Number = 112001006
*
* Product     = MessengerApp
* 
* Project     = MessengerDashboard
*
* Description =  Represents the result of a sentiment analysis for a collection of chat messages.
* 
*****************************************************************************/
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
        public int PositiveChatCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the count of chat messages with negative sentiment.
        /// </summary>
        public int NegativeChatCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the count of chat messages with negative sentiment.
        /// </summary>
        public int NeutralChatCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating the overall sentiment.
        /// </summary>
        /// <remarks>
        /// Can be Positive, Negative or Neutral.
        /// </remarks>
        public string OverallSentiment { get; set; } = "Positive";
    }
}
