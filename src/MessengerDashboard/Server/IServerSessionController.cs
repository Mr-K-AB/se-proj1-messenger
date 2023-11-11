using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Server.Events;
using MessengerNetworking.NotificationHandler;

namespace MessengerDashboard.Server
{
    /// <summary>
    /// Provides an interface for a server session handler.
    /// </summary>
    public interface IServerSessionController : INotificationHandler
    {
        /// <summary>
        /// Fires when session is updated.
        /// </summary>
        event EventHandler<SessionUpdatedEventArgs> SessionUpdated;

    }
}
