using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerCloud;
using MessengerDashboard.Summarization;
using MessengerDashboard.Telemetry;
using MessengerDashboard.UI.DataModels;
using MessengerDashboard.UI.ViewModels;

namespace MessengerDashboard.UI.Commands
{
    public class ExpandCommand : ICommand
    {

        private readonly SessionsViewModel _sessionsViewModel;
        private readonly EntityInfoWrapper _entity;

        public ExpandCommand(SessionsViewModel sessionsViewModel, EntityInfoWrapper entity)
        {
            _sessionsViewModel = sessionsViewModel;
            _entity = entity;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            List<TimeStampChatCountEntry> timeStampChatCountEntries = new();
            foreach(KeyValuePair<DateTime, int> item in _entity.Analysis.TimeStampToUserCountMap)
            {
                timeStampChatCountEntries.Add(new(item.Key, item.Value));
            }

            List<UserActivityEntry> userActivities = new();
            foreach (KeyValuePair<int, UserActivityCloud> item in _entity.Analysis.UserIdToUserActivityMap)
            {
                userActivities.Add(new(item.Key, item.Value.UserChatCount, item.Value.UserName, item.Value.UserEmail,
                                item.Value.EntryTime, item.Value.ExitTime));
            }
            _sessionsViewModel.PositiveChatCount = _entity.PositiveChatCount;
            _sessionsViewModel.NegativeChatCount = _entity.NegativeChatCount;
            _sessionsViewModel.TotalChatCount = _entity.Analysis.TotalChatCount;
            _sessionsViewModel.OverallSentiment = _entity.IsOverallSentimentPositive ? "Positive" : "Negative";
            _sessionsViewModel.TotalUserCount = _entity.Analysis.TotalUserCount;
            _sessionsViewModel.SessionSummary = string.Join(Environment.NewLine, _entity.Sentences);
            _sessionsViewModel.TimeStampChatCountEntries = timeStampChatCountEntries;
            _sessionsViewModel.UserActivities = userActivities;
        }
    }
}
