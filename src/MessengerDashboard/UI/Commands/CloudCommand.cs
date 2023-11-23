using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerCloud;
using MessengerDashboard.UI.DataModels;
using MessengerDashboard.UI.ViewModels;

namespace MessengerDashboard.UI.Commands
{
    public class CloudCommand : ICommand
    {
        private readonly RestClient _restClient;

        private readonly SessionsViewModel _sessionsViewModel;

        public CloudCommand(RestClient restClient, SessionsViewModel sessionsViewModel)
        {
            _restClient = restClient;
            _sessionsViewModel = sessionsViewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _sessionsViewModel.IsLocalClicked = false;
            Thread something = new(() =>
            {
                try
                {
                    Task<IReadOnlyList<Entity>?> task = _restClient.GetEntitiesAsync();
                    task.Wait();
                    IReadOnlyList<Entity> results = task.Result;
                    List<EntityInfoWrapper> entities = new();
                    foreach (Entity entity in results)
                    {
                        entities.Add(new(entity.Sentences, entity.PositiveChatCount, entity.NegativeChatCount, entity.NeutralChatCount, entity.OverallSentiment, entity.SessionId, entity.Analysis));
                    }
                    List<SessionEntry> sessions = new();
                    foreach (EntityInfoWrapper entity in entities)
                    {
                        sessions.Add(new SessionEntry(entity.SessionId, new ExpandCommand(_sessionsViewModel, entity)));
                    }
                    _sessionsViewModel.Sessions = sessions;
                }
                catch (Exception ex) { }
            })
            { IsBackground = true };
            something.Start();
            something.Join();
        }
    }
}
