using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using MessengerCloud;
using MessengerDashboard.Client;
using MessengerDashboard.Sentiment;
using MessengerDashboard.Summarization;
using MessengerDashboard.Telemetry;
using MessengerDashboard.UI.Commands;
using MessengerDashboard.UI.DataModels;

namespace MessengerDashboard.UI.ViewModels
{
    public class DashboardMemberViewModel : DashboardViewModel
    {
        public DashboardMemberViewModel()
        {
            _client.SessionChanged += HandleSessionChanged;
            _client.TelemetryAnalysisChanged += HandleTelemetryAnalysisChanged;
            _client.SummaryChanged += HandleSummaryChanged;
            _client.SentimentChanged += HandleSentimentChanged;
            Mode = (_client.SessionInfo.SessionMode == SessionMode.Exam) ? "Exam" : "Lab";
        }
    }
}
