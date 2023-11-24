/******************************************************************************
* Filename    = TextSummarizerFactory.cs
*
* Author      = Pratham Ravindra Nagpure
*
* Roll number = 112001054
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = This file contains a factory for summarization instances.
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Provides a factory for creating and obtaining an instance of an <see cref="ITextSummarizer"/> implementation.
/// </summary>
namespace MessengerDashboard.Summarization
{
    public static class TextSummarizerFactory
    {
        private static readonly Lazy<TextRankSummarizer> s_textRankSummarizer = new(() => new TextRankSummarizer());

        /// <summary>
        /// Gets an instance of an object that implements the <see cref="ITextSummarizer"/> interface.
        /// </summary>
        /// <returns>An instance of an object capable of summarizing text based on the TextRank algorithm.</returns>
        public static ITextSummarizer GetTextSummarizer()
        {
            return s_textRankSummarizer.Value;
        }
    }
}
