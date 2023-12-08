/******************************************************************************
* Filename    = ITextSummarizer.cs
*
* Author      = Pratham Ravindra Nagpure
*
* Roll number = 112001054
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = This file contains an interface for text summarizer.
*****************************************************************************/

namespace MessengerDashboard.Summarization
{
    /// <summary>
    /// Provides an interface for text summarization.
    /// </summary>
    public interface ITextSummarizer
    {
        /// <summary>
        /// Summarizes the sentences given the options
        /// </summary>
        /// <param name="sentences">Sentences to summarize</param>
        /// <param name="options">Options for summarization</param>
        /// <returns></returns>
        TextSummary Summarize(string[] sentences, TextSummarizationOptions options);
    }
}
