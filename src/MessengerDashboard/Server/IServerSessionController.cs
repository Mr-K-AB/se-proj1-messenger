/******************************************************************************
* Filename    = IServerSessionController.cs
*
* Author      = Pratham Ravindra Nagpure 
*
* Roll number = 112001054
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = Provides an interface for a server session controller.
*****************************************************************************/

using System;
using MessengerDashboard.Server.Events;

namespace MessengerDashboard.Server
{
    /// <summary>
    /// Provides an interface for a server session controller.
    /// </summary>
    public interface IServerSessionController
    {
        /// <summary>
        /// Gets the connection details for the server session.
        /// </summary>
        ConnectionDetails ConnectionDetails { get; }

        /// <summary>
        /// Event triggered when the server session is updated.
        /// </summary>
        event EventHandler<SessionUpdatedEventArgs>? SessionUpdated;
    }
}
