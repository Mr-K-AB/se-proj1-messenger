/************************************************************
* Filename    = ScreenshareServerViewModel.cs
*
* Author      = Likhith Reddy
*
* Product     = ScreenShare
* 
* Project     = Messenger
*
* Description = Contains view model for screenshare client.
************************************************************/

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

        private DispatcherOperation? _sharingScreenOp, _displayPopupOperation;

        /// <summary>
        /// Whether the popup is open or not.
        /// </summary>
        private bool _isPopupOpen;

        /// <summary>
        /// The text to be displayed on the popup.
        /// </summary>
        private string _popupText;

        /// <summary>
        /// Gets the dispatcher to the main thread. In case it is not available (such as during
        /// unit testing) the dispatcher associated with the current thread is returned.
        /// </summary>
        private Dispatcher ApplicationMainThreadDispatcher =>
            (Application.Current?.Dispatcher != null) ?
                    Application.Current.Dispatcher :
                    Dispatcher.CurrentDispatcher;


        /// <summary>
        /// Gets whether the popup is open or not.
        /// </summary>
        public bool IsPopupOpen
        {
            get => _isPopupOpen;

            // Don't keep the setter private, as it is bind using two-way binding.
            set
            {
                if (_isPopupOpen != value)
                {
                    _isPopupOpen = value;
                    OnPropertyChanged(nameof(IsPopupOpen));
                }
            }
        }

        /// <summary>
        /// Gets the text to be displayed on the popup.
        /// </summary>
        public string PopupText
        {
            get => _popupText;

            private set
            {
                if (_popupText != value)
                {
                    _popupText = value;
                    OnPropertyChanged(nameof(PopupText));
                }
            }
        }


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
                    Task.Run(async () => await _model.StartScreensharingAsync());
                }
                else
                {
                    _model.StopScreensharing();
                }
            }
        }

        /// <summary>
        /// Used to display the popup on the UI with the given message.
        /// </summary>
        /// <param name="message">
        /// Message to be displayed on the popup.
        /// </param>
        public void DisplayPopup(string message)
        {
            _displayPopupOperation = ApplicationMainThreadDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action<string>((text) =>
                {
                    lock (this)
                    {
                        // Close the popup if it was already opened before.
                        if (IsPopupOpen)
                        {
                            IsPopupOpen = false;
                        }

                        PopupText = text;
                        IsPopupOpen = true;
                    }
                }),
                message
            );
        }

        /// <summary>
        /// Constructor for the ScreenshareClientViewModel.
        /// </summary>
        public ScreenshareClientViewModel()
        {
            _model = ScreenshareFactory.ScreenshareFactory.getClientInstance(this);
            _sharingScreen = false;
            _isPopupOpen = false;
            _popupText = "";
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
