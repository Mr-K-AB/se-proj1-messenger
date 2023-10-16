using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTextSummarizer;
using OpenTextSummarizer.Interfaces;

namespace MessengerDashboard.Summarization
{
    public class TextSummarizer : ITextSummarizer
    {
        public TextSummary Summarize(TextSummarizationRequest request)
        {
            IContentProvider textContent = new DirectTextContentProvider(request.Text);
            SummarizedDocument summarizedDocument = Summarizer.Summarize(textContent ,
            new SummarizerArguments()
            {
                Language = "en" ,
                MaxSummarySentences = request.MaxSummarySentences,
                MaxSummarySizeInPercent = request.MaxSummaryPercentage
            });
            return new TextSummary(summarizedDocument.Sentences);
        }
    }
}
