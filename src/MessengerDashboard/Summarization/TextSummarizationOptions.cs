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
        public int MaxSummarySentences { get; set; } = 15;

        /// <summary>
        /// Gets or sets the maximum percentage of the original text length for the summary.
        /// </summary>
        public int MaxSummaryPercentage { get; set; } = 10;
    }
}
