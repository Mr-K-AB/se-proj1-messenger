/******************************************************************************
* Filename    = ExpandCommand.cs
*
* Author      = Satish Patidar 
*
* Roll number = 112001037
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = A Command for expanding sessions in the Messenger Dashboard.
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerDashboard.UI.DataModels;
using MessengerDashboard.UI.ViewModels;
using MessengerCloud;
using System.Threading;

namespace MessengerDashboard.UI.Commands
{
    /// <summary>
    /// Represents a command for expanding sessions in the Messenger Dashboard.
    /// </summary>
    public class ExpandCommand : ICommand
    {
        private readonly SessionsViewModel _sessionsViewModel;
        private EntityInfoWrapper _entity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandCommand"/> class.
        /// </summary>
        /// <param name="sessionsViewModel">The view model for sessions.</param>
        /// <param name="entity">The entity information wrapper.</param>
        public ExpandCommand(SessionsViewModel sessionsViewModel, EntityInfoWrapper entity)
        {
            _sessionsViewModel = sessionsViewModel;
            _entity = entity;
        }

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <inheritdoc/>
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <inheritdoc/>
        public void Execute(object? parameter)
        {
            if (_sessionsViewModel.IsLocalClicked == false)
            {
                Thread something = new(() =>
                {
                    try
                    {
                        Task<Entity?> task = _sessionsViewModel.RestClient.GetEntityAsync(_entity.SessionId);
                        task.Wait();
                        Entity? result = task.Result;
                        if (result == null)
                        {
                            return;
                        }
                        _entity = new EntityInfoWrapper(result.Sentences, result.PositiveChatCount, result.NegativeChatCount,
                            result.NeutralChatCount, result.OverallSentiment, result.SessionId, result.Analysis);
                    }
                    catch (Exception ex) { /* Handle exceptions */ }
                })
                { IsBackground = true };
                something.Start();
                something.Join();
            }
            try
            {
                List<TimeStampUserCountEntry> timeStampUserCountEntries = new();
                foreach (KeyValuePair<DateTime, int> item in _entity.Analysis.TimeStampToUserCountMap)
                {
                    timeStampUserCountEntries.Add(new(item.Key, item.Value));
                }

                List<UserActivityEntry> userActivities = new();
                foreach (KeyValuePair<int, MessengerCloud.UserActivity> item in _entity.Analysis.UserIdToUserActivityMap)
                {
                    userActivities.Add(new(item.Key, item.Value.UserChatCount, item.Value.UserName, item.Value.UserEmail,
                                    item.Value.EntryTime, item.Value.ExitTime));
                }
                _sessionsViewModel.PositiveChatCount = _entity.PositiveChatCount;
                _sessionsViewModel.NegativeChatCount = _entity.NegativeChatCount;
                _sessionsViewModel.NeutralChatCount = _entity.NeutralChatCount;
                _sessionsViewModel.OverallSentiment = _entity.OverallSentiment;
                _sessionsViewModel.TotalUserCount = _entity.Analysis.TotalUserCount;
                _sessionsViewModel.SessionSummary = string.Join(Environment.NewLine, _entity.Sentences);
                _sessionsViewModel.TimeStampUserCountEntries = timeStampUserCountEntries;
                _sessionsViewModel.UserActivities = userActivities;
            }
            catch (Exception ex) { /* Handle exceptions */ }
        }
    }
}
