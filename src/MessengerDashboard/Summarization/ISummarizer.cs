/// <credits>
/// <author>
/// <name>Pratham Ravindra Nagpure</name>
/// <rollnumber>112001054</rollnumber>
/// </author>
/// </credits>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Summarization
{
    /// <summary>
    /// Provides an interface for text summarization.
    /// </summary>
    public interface ISummarizer
    {
        string Summarize(string text);
    }
}
