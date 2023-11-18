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
            _client.SessionExited += HandleSessionExited;
        }

        protected void HandleSessionExited(object? sender, Client.Events.SessionExitedEventArgs e)
        {
            lock (this)
            {
                EntityInfoWrapper entity = CreateSessionSaveData(e.Summary, e.Sentiment, e.TelemetryAnalysis);
                if (IsLocalSavingEnabled)
                {
                    SaveSessionToLocalStorage(entity);
                }
            }
        }
    }
}
