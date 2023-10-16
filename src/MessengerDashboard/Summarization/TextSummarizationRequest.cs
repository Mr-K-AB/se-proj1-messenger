using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Summarization
{
    /// <summary>
    /// Represents a request for text summarization.
    /// </summary>
    public class TextSummarizationRequest
    {
        /// <summary>
        /// Gets or sets the text to be summarized.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="string.Empty"/>.
        /// </remarks>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the maximum number of sentences in the summary.
        /// </summary>
        /// <remarks>
        /// The default value is 15.
        /// </remarks>
        public int MaxSummarySentences { get; set; } = 15;

        /// <summary>
        /// Gets or sets the maximum percentage of the original text in the summary.
        /// </summary>
        /// <remarks>
        /// The default value is 10.
        /// </remarks>
        public int MaxSummaryPercentage { get; set; } = 10;
    }
}

