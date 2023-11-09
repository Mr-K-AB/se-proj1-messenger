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
        public AnalysisResults? AnalysisResults { get; set; }

        public AnalyticsChangedEventArgs(AnalysisResults? analysisResults)
        {
            AnalysisResults = analysisResults;
        }
    }
}
