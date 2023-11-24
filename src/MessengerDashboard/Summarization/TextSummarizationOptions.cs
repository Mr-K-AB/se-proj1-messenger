/******************************************************************************
* Filename    = TextSummarizationOptions.cs
*
* Author      = Pratham Ravindra Nagpure
*
* Roll number = 112001054
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = This file contains options for summarization.
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Summarization
{
    /// <summary>
    /// Represents options for text summarization, allowing customization of summarization parameters.
    /// </summary>
    public class TextSummarizationOptions
    {
        /// <summary>
        /// Gets or sets the maximum number of sentences in the summary.
        /// </summary>
        public int MaxSummarySentences { get; set; } = 20;

        /// <summary>
        /// Gets or sets the maximum percentage of the original text length for the summary.
        /// </summary>
        public int MaxSummaryPercentage { get; set; } = 80;
    }
}
