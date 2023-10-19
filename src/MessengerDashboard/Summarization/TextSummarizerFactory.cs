using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Summarization
{
    public static class TextSummarizerFactory
    {
        private static readonly Lazy<TextRankSummarizer> s_textRankSummarizer = new(() => new TextRankSummarizer());

        public static ITextSummarizer GetTextSummarizer()
        {
            return s_textRankSummarizer.Value;
        }
    }
}
