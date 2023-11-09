using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Telemetry;

namespace MessengerDashboard.Client.Events
{
    public class AnalyticsChangedEventArgs
    {
        public Analysis? AnalysisResults { get; set; }

        public AnalyticsChangedEventArgs(Analysis? analysisResults)
        {
            AnalysisResults = analysisResults;
        }
    }
}
