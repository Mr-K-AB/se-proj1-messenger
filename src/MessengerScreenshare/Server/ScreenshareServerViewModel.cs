/*******************************************************************************************
* Filename    = ScreenshareServerViewModel.cs
*
* Author      = Likhith Reddy
*
* Product     = ScreenShare
* 
* Project     = Messenger
*
* Description = This represents the view model for screen sharing on the server side machine.
********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TraceLogger;

namespace MessengerScreenshare.Server
{
    /// <summary>
    /// Represents the view model for screen sharing on the server side machine.
    /// </summary>
    public class ScreenshareServerViewModel :
        INotifyPropertyChanged, // Notifies the UX that a property value has changed.
        IDataReceiver,       // Notifies the UX that subscribers list has been updated.
        IDisposable             // Handle cleanup work for the allocated resources.
    {
        /// <summary>
        /// The only singleton instance for this class.
        /// </summary>
        private static ScreenshareServerViewModel? s_instance;

        /// <summary>
        /// Underlying data model.
        /// </summary>
        private readonly ScreenshareServer? _model;

        /// <summary>
        /// List of all the clients sharing their screens. This list first contains
        /// the clients which are marked as pinned and then the rest of the clients
        /// in lexicographical order of their name.
        /// </summary>
        private List<SharedClientScreen> _subscribers;

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The clients which are on the current view.
        /// </summary>
        private readonly ObservableCollection<SharedClientScreen> _currentClients;

        /// <summary>
        /// The current number of rows of the grid displayed on the screen.
        /// </summary>
        private int _currentRows;

        /// <summary>
        /// The current number of columns of the grid displayed on the screen.
        /// </summary>
        private int _currentColumns;

        /// <summary>
        /// Whether the popup is open or not.
        /// </summary>
        private bool _isPopupOpen;

        /// <summary>
        /// The text to be displayed on the popup.
        /// </summary>
        private string _popupText;

        /// <summary>
        /// The dispatcher operation returned from the calls to BeginInvoke.
        /// </summary>
        /// <remarks>
        /// They are only required for unit tests.
        /// </remarks>
        private DispatcherOperation? _updateViewOperation, _displayPopupOperation;

        /// <summary>
        /// Creates an instance of the "ScreenshareServerViewModel" which represents the
        /// view model for screen sharing on the server side. It also instantiates the instance
        /// of the underlying data model.
        /// </summary>
        /// <param name="isDebugging">
        /// If we are in debugging mode.
        /// </param>
        public ScreenshareServerViewModel(bool isDebugging)
        {
            // Get the instance of the underlying data model.
            _model = ScreenshareServer.GetInstance(this, isDebugging);

            // Initialize rest of the fields.
            _subscribers = new();
            _disposed = false;
            _currentClients = new();
            _currentRows = InitialNumberOfRows;
            _currentColumns = InitialNumberOfCols;
            _isPopupOpen = false;
            _popupText = "";

            Logger.Log("Successfully created an instance for the view model", LogLevel.INFO);
        }

        /// <summary>
        /// Destructor for the class that will perform some cleanup tasks.
        /// This destructor will run only if the Dispose method does not get called.
        /// It gives the class the opportunity to finalize.
        /// </summary>
        ~ScreenshareServerViewModel()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(disposing: false) is optimal in terms of
            // readability and maintainability.
            Dispose(disposing: false);
        }

        /// <summary>
        /// Property changed event raised when a property is changed on a component.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifies that subscribers list has been changed.
        /// This will happen when a client either starts or stops screen sharing.
        /// </summary>
        /// <param name="subscribers">
        /// Updated list of clients.
        /// </param>
        public void OnSubscribersUpdated(List<SharedClientScreen> subscribers)
        {
            Debug.Assert(subscribers != null, Utils.GetDebugMessage("Received null subscribers list"));

            // Acquire lock because timer threads could also execute simultaneously.
            lock (_subscribers)
            {
                // Move the subscribers marked as pinned to the front of the list
                // keeping the lexicographical order of their name.
                _subscribers = RearrangeSubscribers(subscribers);
            }

            // Recompute the current  clients to notify the UX.
            RecomputeCurrentClients();

            Logger.Log($"Successfully updated the subscribers list", LogLevel.INFO);
        }

        /// <summary>
        /// Notifies that a client has started screen sharing.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client who started screen sharing.
        /// </param>
        /// <param name="clientName">
        /// Name of the client who started screen sharing.
        /// </param>
        public void OnScreenshareStart(int clientId, string clientName)
        {
            if (clientName == "")
            {
                return;
            }

            Logger.Log($"{clientName} with Id {clientId} has started screen sharing", LogLevel.INFO);
            DisplayPopup($"{clientName} has started screen sharing");
        }

        /// <summary>
        /// Notifies that a client has stopped screen sharing.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client who stopped screen sharing.
        /// </param>
        /// <param name="clientName">
        /// Name of the client who stopped screen sharing.
        /// </param>
        public void OnScreenshareStop(int clientId, string clientName)
        {
            if (clientName == "")
            {
                return;
            }

            Logger.Log($"{clientName} with Id {clientId} has stopped screen sharing", LogLevel.INFO);
            DisplayPopup($"{clientName} has stopped screen sharing");
        }

        /// <summary>
        /// Implement "IDisposable". Disposes the managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, we should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Constants for initial view.
        public static int InitialNumberOfRows { get; } = 1;
        public static int InitialNumberOfCols { get; } = 1;

        /// <summary>
        /// Gets the maximum number of tiles of the shared screens
        /// on a single screen that will be shown to the server.
        /// </summary>
        public static int MaxTiles { get; } = 20;

        /// <summary>
        /// Acts as a map from the number of screens on the current  to
        /// the number of rows and columns of the grid displayed on the screen.
        /// </summary>
        public static List<(int Row, int Column)> NumRowsColumns { get; } = new()
        {
            (1, 1),  // 0 Total Screen.
            (1, 1),  // 1 Total Screen.
            (1, 2),  // 2 Total Screens.
            (2, 2),  // 3 Total Screens.
            (2, 2),  // 4 Total Screens.
            (2, 3),  // 5 Total Screens.
            (2, 3),  // 6 Total Screens.
            (3, 3),  // 7 Total Screens.
            (3, 3),  // 8 Total Screens.
            (3, 3),   // 9 Total Screens.
            (3, 4),
            (3, 4),
            (3, 4),
            (3, 5),
            (3, 5),
            (3, 5),
            (3, 6),
            (3, 6),
            (3, 6),
            (3, 7),
            (3, 7)
        };

        /// <summary>
        /// Gets the clients which are on the current view.
        /// </summary>
        public ObservableCollection<SharedClientScreen> CurrentClients
        {
            get => _currentClients;

            private set
            {
                // Note, to update the whole list, we can't simply assign it equal
                // to the new list. We need to clear the list first and add new elements
                // into the list to be able to see the changes on the UI.
                _currentClients.Clear();
                foreach (SharedClientScreen screen in value)
                {
                    _currentClients.Add(screen);
                }
                OnPropertyChanged(nameof(CurrentClients));
            }
        }

        /// <summary>
        /// Gets the current number of rows of the grid displayed on the screen.
        /// </summary>
        public int CurrentRows
        {
            get => _currentRows;

            private set
            {
                if (_currentRows != value)
                {
                    _currentRows = value;
                    OnPropertyChanged(nameof(CurrentRows));
                }
            }
        }

        /// <summary>
        /// Gets the current number of columns of the grid displayed on the screen.
        /// </summary>
        public int CurrentColumns
        {
            get => _currentColumns;

            private set
            {
                if (_currentColumns != value)
                {
                    _currentColumns = value;
                    OnPropertyChanged(nameof(CurrentColumns));
                }
            }
        }

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
        /// Gets the dispatcher to the main thread. In case it is not available
        /// (such as during unit testing) the dispatcher associated with the
        /// current thread is returned.
        /// </summary>
        public static Dispatcher ApplicationMainThreadDispatcher =>
            (Application.Current?.Dispatcher != null) ?
                    Application.Current.Dispatcher :
                    Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Gets a singleton instance of "ScreenshareServerViewModel" class.
        /// </summary>
        /// <param name="isDebugging">
        /// If we are in debugging mode.
        /// </param>
        /// <returns>
        /// A singleton instance of "ScreenshareServerViewModel".
        /// </returns>
        public static ScreenshareServerViewModel GetInstance(bool isDebugging = false)
        {
            // Create a new instance if it was null before.
            s_instance ??= new(isDebugging);
            return s_instance;
        }

        /// <summary>
        /// Recomputes current  clients using the pagination logic
        /// and notifies the UX. It also notifies the new clients
        /// about the new status of sending image packets.
        /// </summary>
        public void RecomputeCurrentClients()
        {
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));

            // Reset the view if the subscribers count is zero.
            if (_subscribers.Count == 0)
            {
                // Update the view to its initial state.
                UpdateView(
                    new(),
                    InitialNumberOfRows,
                    InitialNumberOfCols,
                    (InitialNumberOfRows, InitialNumberOfCols)
                );

                // Notifies the current clients to stop sending their image packets.
                NotifySubscribers(new(), (InitialNumberOfRows, InitialNumberOfCols));
                return;
            }

            int newNumRows = 1, newNumCols = 1;
            List<SharedClientScreen> newClients;
            (int Height, int Width) newTileDimensions = (0, 0);

            lock (_subscribers)
            {
                int numNewClients = _subscribers.Count;

                // The new number of rows and columns to be displayed based on new number of clients.
                (newNumRows, newNumCols) = NumRowsColumns[numNewClients];

                newClients = _subscribers;

                // The new tile dimensions of screen image to be displayed based on new number of clients.
                newTileDimensions = GetTileDimensions(newNumRows, newNumCols);
            }

            // Update the view with the new fields.
            UpdateView(
                newClients,
                newNumRows,
                newNumCols,
                newTileDimensions
            );

            // Notifies the new clients about the status of sending image packets.
            NotifySubscribers(newClients, (newNumRows, newNumCols));

            Logger.Log($"Successfully recomputed current clients", LogLevel.INFO);
        }

        /// <summary>
        /// Mark the client as pinned and switch to that client's tile.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client which is marked as pinned.
        /// </param>
        public void OnPin(int clientId)
        {
            Debug.Assert(clientId != 0, Utils.GetDebugMessage("Received null client id"));
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));
            Debug.Assert(_subscribers.Count != 0, Utils.GetDebugMessage("_subscribers has count 0"));

            // Acquire lock because timer threads could also execute simultaneously.
            lock (_subscribers)
            {
                // Find the index of the client.
                int pinnedScreenIdx = _subscribers.FindIndex(subscriber => subscriber.Id == clientId);

                Debug.Assert(pinnedScreenIdx != -1, Utils.GetDebugMessage($"Client Id: {clientId} not found in the subscribers list"));

                // If client not found.
                if (pinnedScreenIdx == -1)
                {
                    Logger.Log($"Client Id: {clientId} not found in the subscribers list", LogLevel.INFO);
                    return;
                }

                // Mark the client as pinned.
                SharedClientScreen pinnedScreen = _subscribers[pinnedScreenIdx];
                pinnedScreen.Pinned = true;

                // Move the subscribers marked as pinned to the front of the list
                // keeping the lexicographical order of their name.
                _subscribers = RearrangeSubscribers(_subscribers);
            }

            // Switch to the view of the client.
            RecomputeCurrentClients();

            Logger.Log($"Successfully pinned the client with id: {clientId}", LogLevel.INFO);
        }

        /// <summary>
        /// Mark the client as unpinned and switch to the previous view.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client which is marked as unpinned.
        /// </param>
        public void OnUnpin(int clientId)
        {
            Debug.Assert(clientId != 0, Utils.GetDebugMessage("Received null client id"));
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));
            Debug.Assert(_subscribers.Count != 0, Utils.GetDebugMessage("_subscribers has count 0"));

            // Acquire lock because timer threads could also execute simultaneously.
            lock (_subscribers)
            {
                // Find the index of the client.
                int unpinnedScreenIdx = _subscribers.FindIndex(subscriber => subscriber.Id == clientId);

                Debug.Assert(unpinnedScreenIdx != -1, Utils.GetDebugMessage($"Client Id: {clientId} not found in the subscribers list"));

                // If client not found.
                if (unpinnedScreenIdx == -1)
                {
                    Logger.Log($"Client Id: {clientId} not found in the subscribers list", LogLevel.INFO);
                    return;
                }

                // Mark the client as unpinned.
                SharedClientScreen unpinnedScreen = _subscribers[unpinnedScreenIdx];
                unpinnedScreen.Pinned = false;

                // Move the subscribers marked as pinned to the front of the list
                // keeping the lexicographical order of their name.
                _subscribers = RearrangeSubscribers(_subscribers);
            }

            //  Switch to the previous view.
            RecomputeCurrentClients();

            Logger.Log($"Successfully unpinned the client with id: {clientId}", LogLevel.INFO);
        }

        /// <summary>
        /// It executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the destructor and we should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing">
        /// Indicates if we are disposing this object.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (_disposed)
            {
                return;
            }

            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if (disposing)
            {
                _model?.Dispose();
                _subscribers.Clear();
                s_instance = null;
            }

            // Call the appropriate methods to clean up unmanaged resources here.

            // Now disposing has been done.
            _disposed = true;
        }

        /// <summary>
        /// Moves the subscribers marked as pinned to the front of the list
        /// keeping the lexicographical order of their name.
        /// </summary>
        /// <param name="subscribers">
        /// Input list of subscribers.
        /// </param>
        /// <returns>
        /// List of subscribers with pinned subscribers in front.
        /// </returns>
        private static List<SharedClientScreen> MovePinnedSubscribers(List<SharedClientScreen> subscribers)
        {
            Debug.Assert(subscribers != null, Utils.GetDebugMessage("Received null subscribers list"));

            // Separate pinned and unpinned subscribers.
            List<SharedClientScreen> pinnedSubscribers = new();
            List<SharedClientScreen> unpinnedSubscribers = new();

            foreach (SharedClientScreen subscriber in subscribers)
            {
                if (subscriber.Pinned)
                {
                    pinnedSubscribers.Add(subscriber);
                }
                else
                {
                    unpinnedSubscribers.Add(subscriber);
                }
            }

            // Join both the lists with pinned subscribers followed by the unpinned ones.
            return pinnedSubscribers.Concat(unpinnedSubscribers).ToList();
        }

        /// <summary>
        /// Rearranges the subscribers list by first having the Pinned subscribers followed by
        /// the unpinned subscribers. The pinned and unpinned subscribers are kept in the
        /// lexicographical order of their names.
        /// </summary>
        /// <param name="subscribers">
        /// The subscribers list to rearrange.
        /// </param>
        /// <returns>
        /// The rearranged subscribers list.
        /// </returns>
        private static List<SharedClientScreen> RearrangeSubscribers(List<SharedClientScreen> subscribers)
        {
            // Sort the subscribers in lexicographical order of their name.
            List<SharedClientScreen> sortedSubscribers = subscribers
                                                            .OrderBy(subscriber => subscriber.Name)
                                                            .ToList();

            // Move the subscribers marked as pinned to the front of the list
            // keeping the lexicographical order of their name.
            return MovePinnedSubscribers(sortedSubscribers);
        }

        /// <summary>
        /// Gets the tile dimensions in the grid displayed on the screen
        /// based on the number of rows and columns presented.
        /// </summary>
        /// <param name="rows">
        /// Number of rows of the grid on the screen.
        /// </param>
        /// <param name="columns">
        /// Number of columns of the grid on the screen.
        /// </param>
        /// <returns>
        /// A tuple having the height and width of the each grid tile.
        /// </returns>
        private static (int Height, int Width) GetTileDimensions(int rows, int columns)
        {
            // Get the total system height and width.
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double screenWidth = SystemParameters.PrimaryScreenWidth;

            // The margins which are kept on the UI.
            double marginBetweenImages = (rows + 1) * 6;
            double otherMargins = 100;

            // Compute the tile height and width.
            double remainingHeight = screenHeight - marginBetweenImages - otherMargins;
            int tileHeight = (int)Math.Floor(remainingHeight / rows);
            int tileWidth = (int)Math.Floor(screenWidth / columns);

            Debug.Assert(tileHeight >= 0, Utils.GetDebugMessage("Tile Height should be non-negative"));
            Debug.Assert(tileWidth >= 0, Utils.GetDebugMessage("Tile Width should be non-negative"));

            return (tileHeight, tileWidth);
        }

        /// <summary>
        /// Starts the processing task for the clients.
        /// </summary>
        /// <param name="clients">
        /// The clients for which the processing task needs to be started.
        /// </param>
        private static void StartProcessingForClients(List<SharedClientScreen> clients)
        {
            try
            {
                // Ask all the current  clients to start processing their images.
                foreach (SharedClientScreen client in clients)
                {
                    // The lambda function takes the final image from the final image queue
                    // of the client and set it as the "CurrentImage" variable for the client
                    // and notify the UX about the same.
                    client.StartProcessing(new Action<int>((taskId) =>
                    {
                        // Loop till the task is not canceled.
                        while (client.TaskId == taskId)
                        {
                            try
                            {
                                // Get the final image to be displayed on the UI.
                                Bitmap? finalImage = client.GetFinalImage(taskId);

                                if (finalImage == null)
                                {
                                    continue;
                                }

                                // Update the current image of the client on the screen
                                // by taking the processed images from its final image queue.
                                _ = ApplicationMainThreadDispatcher.BeginInvoke(
                                        DispatcherPriority.Normal,
                                        new Action<Bitmap>((image) =>
                                        {
                                            if (image != null)
                                            {
                                                BitmapImage img = Utils.BitmapToBitmapImage(image);
                                                img.Freeze();
                                                lock (client)
                                                {
                                                    client.CurrentImage = img;
                                                }
                                            }
                                        }),
                                        finalImage
                                    );
                            }
                            catch (Exception e)
                            {
                                Logger.Log($"Failed to update the view: {e.Message}", LogLevel.INFO);
                            }

                        }
                    }));
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Failed to start the processing: {e.Message}", LogLevel.INFO);
            }
        }

        /// <summary>
        /// Notify the previous/new  clients to stop/send their image packets.
        /// It also asks the previous/new  clients to stop/start their image processing.
        /// </summary>
        /// <param name="currentClients">
        /// List of clients which are there in the current .
        /// </param>
        /// <param name="numRowsColumns">
        /// Number of rows and columns for the resolution of the image to be sent by the current  clients.
        /// </param>
        private void NotifySubscribers(List<SharedClientScreen> currentClients, (int, int) numRowsColumns)
        {
            Debug.Assert(_model != null, Utils.GetDebugMessage("_model is found null"));
            Debug.Assert(currentClients != null, Utils.GetDebugMessage("list of current  clients is null"));

            // Ask all the current  clients to start sending image packets with the specified resolution.
            _model.BroadcastClients(currentClients
                                    .Select(client => client.Id)
                                    .ToList(), nameof(ServerDataHeader.Send), numRowsColumns);

            // Start processing for the current  clients.
            StartProcessingForClients(currentClients);

            Logger.Log("Successfully notified the new current  clients", LogLevel.INFO);
        }

        /// <summary>
        /// Updates the view with the new values provided.
        /// </summary>
        /// <param name="newClients">
        /// The new current  clients.
        /// <param name="newNumRows">
        /// The new number of grid rows on the new view.
        /// </param>
        /// <param name="newNumCols">
        /// The new number of grid columns on the new view.
        /// <param name="newTileDimensions">
        /// The new tile dimension of each grid cell on the new view.
        /// </param>
        private void UpdateView(
            List<SharedClientScreen> newClients,
            int newNumRows,
            int newNumCols,
            (int Height, int Width) newTileDimensions
        )
        {
            // Update all the fields and notify the UX.
            _updateViewOperation = ApplicationMainThreadDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action<
                    ObservableCollection<SharedClientScreen>, int, int, (int Height, int Width)
                >((clients, numRows, numCols, tileDimensions) =>
                {
                    lock (this)
                    {
                        foreach (SharedClientScreen screen in clients)
                        {
                            screen.TileHeight = tileDimensions.Height;
                            screen.TileWidth = tileDimensions.Width;
                        }

                        CurrentClients = clients;
                        CurrentRows = numRows;
                        CurrentColumns = numCols;
                    }
                }),
                new ObservableCollection<SharedClientScreen>(newClients),
                newNumRows,
                newNumCols,
                newTileDimensions
            );
        }

        /// <summary>
        /// Used to display the popup on the UI with the given message.
        /// </summary>
        /// <param name="message">
        /// Message to be displayed on the popup.
        /// </param>
        private void DisplayPopup(string message)
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
        /// Handles the property changed event raised on a component.
        /// </summary>
        /// <param name="property">
        /// The name of the property that is changed.
        /// </param>
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new(property));
        }
    }
}
