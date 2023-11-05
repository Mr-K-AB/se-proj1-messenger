using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Represents a summary of text content, consisting of a collection of sentences.
/// </summary>
namespace MessengerDashboard.Summarization
{
    public class TextSummary
    {
        /// <summary>
        /// Gets a list of sentences that make up the text summary.
        /// </summary>
        public List<string> Sentences { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextSummary"/> class with the provided sentences.
        /// </summary>
        /// <param name="sentences">A list of sentences that form the text summary.</param>
        public TextSummary(List<string> sentences)
        {
            Sentences = sentences;
        }
    }
}
