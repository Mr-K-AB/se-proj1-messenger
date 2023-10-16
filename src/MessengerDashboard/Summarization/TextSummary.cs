using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Summarization
{
    public class TextSummary
    {
        public List<string> Sentences { get; }

        public TextSummary(List<string> sentences)
        {
            Sentences = sentences;
        }
    }
}
