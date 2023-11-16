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

namespace MessengerDashboard.UI.ViewModels
{
    public class DashboardInstructorViewModel : DashboardViewModel
    {
        protected bool _isCloudSavingEnabled;

        public bool IsCloudSavingEnabled
        {
            get => _isCloudSavingEnabled;
            protected set => SetProperty(ref _isCloudSavingEnabled, value);
        }

        protected bool _isLocalSavingEnabled;

        public bool IsLocalSavingEnabled
        {
            get => _isLocalSavingEnabled;
            protected set => SetProperty(ref _isLocalSavingEnabled, value);
        }

        public ICommand SwitchModeCommand { get; } = new SwitchModeCommand();

        public DashboardInstructorViewModel()
        {
            _client.SessionChanged += HandleSessionChanged;
            _client.TelemetryAnalysisChanged += HandleTelemetryAnalysisChanged;
            _client.SummaryChanged += HandleSummaryChanged;
            _client.SentimentChanged += HandleSentimentChanged;
            _client.SessionExited += HandleSessionExited;
            Mode = (_client.SessionInfo.SessionMode == SessionMode.Exam) ? "Exam" : "Lab";
        }

        public EntityInfoWrapper CreateSessionSaveData(TextSummary textSummary, SentimentResult sentimentResult, Analysis analysis)
        {
            EntityInfoWrapper entityInfo = new(textSummary.Sentences, sentimentResult.PositiveChatCount,
                                               sentimentResult.NegativeChatCount, sentimentResult.IsOverallSentimentPositive,
                                               Guid.NewGuid().ToString(), ConvertToCloudObject(analysis));
            return entityInfo;
        }

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

        protected void SaveSessionToLocalStorage(EntityInfoWrapper entityInfo)
        {
            try
            {
                LocalSave.AddEntity(entityInfo);
            }
            catch(Exception e)
            {
                Trace.WriteLine($"{e.Message}");
            }
        }

        protected AnalysisCloud ConvertToCloudObject(Analysis analysis)
        {
            Dictionary<int, UserActivityCloud> userIdToUserActivityMap = new();
            foreach(KeyValuePair<int, UserActivity> keyValuePair in analysis.UserIdToUserActivityMap)
            {
                UserActivityCloud userActivity = new()
                {
                    ExitTime = keyValuePair.Value.ExitTime,
                    EntryTime = keyValuePair.Value.EntryTime,
                    UserEmail = keyValuePair.Value.UserEmail,
                    UserChatCount = keyValuePair.Value.UserChatCount,
                    UserName = keyValuePair.Value.UserName
                };
                userIdToUserActivityMap.Add(keyValuePair.Key, userActivity);
            }
            AnalysisCloud analysisCloud = new(userIdToUserActivityMap, analysis.TimeStampToUserCountMap, analysis.TotalUserCount, analysis.TotalChatCount);
            return analysisCloud;
        }

        protected void HandleSessionExited(object? sender, Client.Events.SessionExitedEventArgs e)
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
