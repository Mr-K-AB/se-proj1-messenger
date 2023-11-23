/******************************************************************************
* Filename    = DashboardInstructorViewModel.cs
*
* Author      = Satish Patidar 
*
* Roll number = 112001037
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = This file contains the implementation of the DashboardInstructorViewModel class, which is a view model for the MessengerDashboard. It provides functionality related to handling sessions, saving data to the cloud, and responding to session exits.
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// ViewModel for the instructor dashboard in the Messenger application.
    /// </summary>
    public class DashboardInstructorViewModel : DashboardViewModel
    {
        private bool _isCloudSavingEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardInstructorViewModel"/> class.
        /// </summary>
        public DashboardInstructorViewModel()
        {
            _client.SessionExited += HandleSessionExited;
            IsCloudSavingEnabled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardInstructorViewModel"/> class with a specified <see cref="IClientSessionController"/>.
        /// </summary>
        /// <param name="client">The client session controller.</param>
        public DashboardInstructorViewModel(IClientSessionController client) : base(client)
        {
            _client.SessionExited += HandleSessionExited;
            IsCloudSavingEnabled = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether cloud saving is enabled.
        /// </summary>
        public bool IsCloudSavingEnabled
        {
            get => _isCloudSavingEnabled;
            set => SetProperty(ref _isCloudSavingEnabled, value);
        }

        /// <summary>
        /// Gets the command to switch mode.
        /// </summary>
        public ICommand SwitchModeCommand { get; } = new SwitchModeCommand();

        /// <summary>
        /// Saves the session data to the cloud.
        /// </summary>
        /// <param name="entityInfo">The session data to save.</param>
        public void SaveSessionToCloud(EntityInfoWrapper entityInfo)
        {
            try
            {
                RestClient restClient = new(_cloudUrl);
                restClient.PostEntityAsync(entityInfo).Wait();
            }
            catch (Exception e)
            {
                Trace.WriteLine($"{e.Message}");
            }
        }

        /// <summary>
        /// Handles the event when a session is exited.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments containing session exit details.</param>
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
