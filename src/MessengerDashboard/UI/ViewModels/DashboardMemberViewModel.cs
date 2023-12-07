/******************************************************************************
* Filename    = DashboardMemberControl.cs
*
* Author      = Satish Patidar 
*
* Roll number = 112001037
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = This file contains the implementation of the DashboardMemberViewModel class, which is a view model for the MessengerDashboard UI. It handles client sessions and provides functionality to save session data locally.
*****************************************************************************/

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
    /// <summary>
    /// Represents the view model for the dashboard used by members in the Messenger application.
    /// </summary>
    public class DashboardMemberViewModel : DashboardViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardMemberViewModel"/> class.
        /// </summary>
        public DashboardMemberViewModel()
        {
            _client.SessionExited += HandleSessionExited;
            _client.SessionModeChanged += HandleSessionModeChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardMemberViewModel"/> class with a specified client session controller.
        /// </summary>
        /// <param name="clientSessionController">The client session controller to be used.</param>
        public DashboardMemberViewModel(IClientSessionController clientSessionController) : base(clientSessionController)
        {
            _client.SessionExited += HandleSessionExited;
            _client.SessionModeChanged += HandleSessionModeChanged;
        }

        private void HandleSessionModeChanged(object? sender, Client.Events.SessionModeChangedEventArgs e)
        {
            if (e.SessionMode == SessionMode.Lab)
            {
                IsVisible = true;
            }
            else
            {
                IsVisible = false;
            }
        }

        /// <summary>
        /// Handles the event when a client session is exited.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments containing session information.</param>
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
            IsDashboardVisible = false;
        }

        protected bool _isVisible = true;

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }
    }
}
