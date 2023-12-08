/******************************************************************************
* Filename    = ISentimentAnalyzer.cs
*
* Author      = Aradhya Bijalwan
*
* Product     = MessengerApp* 
*
* Roll Number = 112001006
* 
* Project     = MessengerDashboard
*
* Description = Implementing interface for SentimentResult
*        
*****************************************************************************/
namespace MessengerDashboard.Sentiment
{
    /// <summary>
    /// Provides an interface for sentiment analysis.
    /// </summary>
    public interface ISentimentAnalyzer
    {
        /// <summary>
        /// Analyzes sentiment in an array of chat messages.
        /// </summary>
        /// <param name="chats"></param>
        /// <returns>The analyzed sentiment.</returns>
        SentimentResult AnalyzeSentiment(string[] chats);
    }
}
