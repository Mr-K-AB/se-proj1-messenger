﻿/// <author> Likhith Reddy </author>
/// <summary>
/// Contains view model for screenshare client.
/// </summary>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace MessengerScreenshare.Client
{
    /// <summary>
    /// ViewModel class for client
    /// </summary>
    public class ScreenshareClientViewModel :
    INotifyPropertyChanged
    {
        // Boolean to store whether the client is sharing screen or not.
        private bool _sharingScreen;

        // Underlying data model for ScreenshareClient.
        private readonly ScreenshareClient _model;

        // Property changed event raised when a property is changed on a component.
        public event PropertyChangedEventHandler? PropertyChanged;

        private DispatcherOperation? _sharingScreenOp;

        /// <summary>
        /// Gets the dispatcher to the main thread. In case it is not available (such as during
        /// unit testing) the dispatcher associated with the current thread is returned.
        /// </summary>
        private Dispatcher ApplicationMainThreadDispatcher =>
            (Application.Current?.Dispatcher != null) ?
                    Application.Current.Dispatcher :
                    Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Boolean to store whether the screen is currently being stored or not.
        /// When the boolen is changed, we call OnPropertyChanged to refresh the view.
        /// We also start/stop the screenshare accordingly when the property is changed.
        /// </summary>
        public bool SharingScreen
        {
            get => _sharingScreen;

            set
            {
                // Execute the call on the application's main thread.
                _sharingScreenOp = ApplicationMainThreadDispatcher.BeginInvoke(
                                    DispatcherPriority.Normal,
                                    new Action(() =>
                                    {
                                        lock (this)
                                        {
                                            _sharingScreen = value;
                                            OnPropertyChanged("SharingScreen");
                                        }
                                    }));

                if (value)
                {
                    string userName = _model.GetUserName();
                    Trace.WriteLine(Utils.GetDebugMessage($"**************Failed to get the name: {userName}*****************", withTimeStamp: true));
                    Task.Run(async () => await _model.StartScreensharingAsync());
                }
                else
                {
                    _model.StopScreensharing();
                }
            }
        }


        /// <summary>
        /// Constructor for the ScreenshareClientViewModel.
        /// </summary>
        public ScreenshareClientViewModel()
        {
            _model = ScreenshareClient.GetInstance(this, false);
            _sharingScreen = false;
        }

        /// <summary>
        /// Handles the property changed event raised on a component.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
