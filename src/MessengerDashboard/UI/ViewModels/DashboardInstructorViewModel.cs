using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerCloud;
using MessengerDashboard.Sentiment;
using MessengerDashboard.Summarization;
using MessengerDashboard.Telemetry;
using MessengerDashboard.UI.Commands;
using MessengerDashboard.UI.DataModels;

namespace MessengerDashboard.UI.ViewModels
{
    public class DashboardInstructorViewModel : DashboardViewModel
    {
        public DashboardInstructorViewModel()
        {
            _client.SessionExited += HandleSessionExited;
            IsCloudSavingEnabled = true;
        }

        protected bool _isCloudSavingEnabled;

        public bool IsCloudSavingEnabled
        {
            get => _isCloudSavingEnabled;
            set => SetProperty(ref _isCloudSavingEnabled, value);
        }


        public ICommand SwitchModeCommand { get; } = new SwitchModeCommand();

        public void SaveSessionToCloud(EntityInfoWrapper entityInfo)
        {
            try
            {
                RestClient restClient = new(_cloudUrl);
                restClient.PostEntityAsync(entityInfo).Wait();
            }
            catch(Exception e)
            {
                Trace.WriteLine($"{e.Message}");
            }
        }

        protected void HandleSessionExited(object? sender, Client.Events.SessionExitedEventArgs e)
        {
            lock (this)
            {
                EntityInfoWrapper entity = CreateSessionSaveData(e.Summary, e.Sentiment, e.TelemetryAnalysis);
                if (IsCloudSavingEnabled)
                {
                    SaveSessionToCloud(entity);
                }
                if (IsLocalSavingEnabled)
                {
                    SaveSessionToLocalStorage(entity);
                }
            }
        }
    }
}
