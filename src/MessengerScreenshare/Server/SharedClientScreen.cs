/// <author>Aditya Raj</author>
/// <summary>
/// This file contains the SharedClientScreen class which 
/// represents the screen shared by a client.
/// </summary>
/// 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

// The Timer class object.
using Timer = System.Timers.Timer;

namespace MessengerScreenshare.Server
{
    public class SharedClientScreen :
        INotifyPropertyChanged, // Notifies the UX that a property value has changed.
        IDisposable             // Handle cleanup work for the allocated resources.
    {
        /// <summary>
        /// Object of Timer which keeps track the time of last packet
        /// received from the client and tells that the client is still
        /// presenting the screen.
        /// </summary>
        private readonly Timer? _timer;

        /// <summary>
        /// The data model defining the callback for the timeout.
        /// </summary>
        private readonly ITimerManager _server;

        /// <summary>
        /// It will store the image receiving from the clients.
        /// </summary>
        private readonly Queue<string> _imageQueue;

        /// <summary>
        /// The screen stitcher associated with this client.
        /// </summary>
        private readonly ScreenStitcher _stitcher;

        /// <summary>
        /// It store the Images which are going to display, i.e., the
        /// final image after stitching the image received from the client.
        /// </summary>
        private readonly Queue<Bitmap> _finalImageQueue;

        /// <summary>
        /// Task which will continuously pick the image from the "_finalImageQueue"
        /// and update the "CurrentImage" variable to continuously update the screen
        /// image of the client. The function for this task will be provided by the
        /// view model which will also invoke "OnPropertyChanged" to notify the UX.
        /// </summary>
        private Task? _imageSendTask;

        /// <summary>
        /// Lock acquired while modifying "_taskId"
        /// </summary>
        private readonly Object _taskIdLock = new();

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Image that is currently displaying on the screen.
        /// </summary>
        private BitmapImage? _currentImage;

        /// <summary>
        /// Whether the client is marked as pinned or not.
        /// </summary>
        private bool _pinned;

        /// <summary>
        /// The height of the tile of the client screen.
        /// </summary>
        private int _tileHeight;

        /// <summary>
        /// The width of the tile of the client screen.
        /// </summary>
        private int _tileWidth;

        /// <summary>
        /// Creates an instance of SharedClientScreen which represents the screen
        /// shared by a client and also stores some other information of the client.
        /// </summary>
        /// <param name="clientId">
        /// The ID of the client.
        /// </param>
        /// <param name="clientName">
        /// The name of the client.
        /// </param>
        /// <param name="server">
        /// The timer manager implementing the callback for the timer object.
        /// </param>
        /// <param name="isDebugging">
        /// If we are in debugging/testing mode.
        /// </param>
        /// <exception cref="Exception"></exception>
        public SharedClientScreen(string clientId, string clientName, ITimerManager server, bool isDebugging = false)
        {
            //Initialize all the variables
            Initialize(clientId, clientName, server);

            if (!isDebugging)
            {
                SetupTimer();
            }

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully created client with id: {this.Id} and name: {this.Name}", withTimeStamp: true));
        }

        /// <summary>
        /// It will initialize all the required variables.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientName"></param>
        /// <param name="server"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private void Initialize(string clientId, string clientName, ITimerManager server)
        {
            this.Id = clientId ?? throw new ArgumentNullException(nameof(clientId));
            this.Name = clientName ?? throw new ArgumentNullException(nameof(clientName));
            _server = server ?? throw new ArgumentNullException(nameof(server));

            // Created a new stitcher object associated to this client.
            _stitcher = new(this);

            // Initialize the queues to be empty.
            _imageQueue = new();
            _finalImageQueue = new();

            // Initially client is not pinned so mark it false.
            _pinned = false;

            // There variables is assigned as null.
            _imageSendTask = null;
            _currentImage = null;


            _disposed = false;
            this.TaskId = 0;
            _tileHeight = 0;
            _tileWidth = 0;
        }

        /// <summary>
        /// It sets up a timer to perform an timeout action,
        /// after a specified time interval.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void SetupTimer()
        {
            try
            {
                _timer = new Timer();
                _timer.Elapsed += (sender, e) => _server.OnTimeOut(sender, e, Id);
                _timer.AutoReset = false;
                UpdateTimer();
                _timer.Enabled = true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to create the timer: {e.Message}" , withTimeStamp: true));
                throw new Exception("Failed to create the timer", e);
            }
        }

        /// <summary>
        /// Destructor for the class that will perform some cleanup tasks.
        /// This destructor will run only if the Dispose method does not get called.
        /// </summary>
        ~SharedClientScreen()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(disposing: false) is optimal in terms of
            // readability and maintainability.
            Dispose( disposing: false );
        }

        /// <summary>
        /// When a property is changed on a component
        /// then property changed event is raised.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Implement IDisposable. Disposes the managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose( disposing: true );

            // This object will be cleaned up by the Dispose method.
            // Therefore, we should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Timeout value for the timer so that it will maximum wait for 
        /// the timeout for the arrival of the packet from the client with 
        /// the confirmation header.
        /// </summary>
        public static double Timeout { get; } = 20 * 1000;

        /// <summary>
        /// Gets the id of the current image sending task.
        /// </summary>
        public int TaskId { get; private set; }

        /// <summary>
        /// Gets the ID of the client sharing this screen.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the name of the client sharing this screen.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the current screen image of the client being displayed.
        /// </summary>
        public BitmapImage? CurrentImage
        {
            get => _currentImage;

            set
            {
                if (_currentImage != value)
                {
                    _currentImage = value;
                    this.OnPropertyChanged(nameof(CurrentImage));
                }
            }
        }

        /// <summary>
        /// Gets whether the client is marked as pinned or not.
        /// </summary>
        public bool Pinned
        {
            get => _pinned;

            set
            {
                if (_pinned != value)
                {
                    _pinned = value;
                    this.OnPropertyChanged(nameof(Pinned));
                }
            }
        }

        /// <summary>
        /// Gets the height of the tile of the client screen.
        /// </summary>
        public int TileHeight
        {
            get => _tileHeight;

            set
            {
                if (_tileHeight != value)
                {
                    _tileHeight = value;
                    this.OnPropertyChanged(nameof(TileHeight));
                }
            }
        }

        /// <summary>
        /// Gets the width of the tile of the client screen.
        /// </summary>
        public int TileWidth
        {
            get => _tileWidth;

            set
            {
                if (_tileWidth != value)
                {
                    _tileWidth = value;
                    this.OnPropertyChanged(nameof(TileWidth));
                }
            }
        }

        /// <summary>
        /// It will pop the recieve message at the beginning 
        /// of the received image queue.
        /// </summary>
        /// <param name="taskId">
        /// Id of the task in which this function is called.
        /// </param>
        /// <returns>
        /// The received image that is removed from the beginning.
        /// </returns>
        public string? GetImage(int taskId)
        {
            lock (_imageQueue)
            {
                // Check if the task is stopped or a new task is started.
                if (_imageQueue == null || taskId != this.TaskId)
                {
                    return null;
                }

                // Use a while loop to wait until the queue is not empty.
                while (_imageQueue.Count == 0)
                {
                    // Release the lock and wait for a signal.
                    Monitor.Wait(_imageQueue);

                    // After being woken up, recheck if the task is stopped or a new task is started.
                    if (_imageQueue == null || taskId != this.TaskId)
                    {
                        return null;
                    }
                }

                Debug.Assert(_imageQueue.Count > 0, Utils.GetDebugMessage("Queue should not be empty"));

                try
                {
                    return _imageQueue.Dequeue();
                }
                catch (InvalidOperationException e)
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Dequeue failed: {e.Message}", withTimeStamp: true));
                    return null;
                }
            }
        }

        /// <summary>
        /// Insert the received image into the received image queue
        /// </summary>
        /// <param name="image">
        /// Image to be inserted
        /// </param>
        /// <param name="taskId">
        /// Id of the task in which this function is called.
        /// </param>
        public void PutImage(string image, int taskId)
        {
            Debug.Assert(_imageQueue != null, Utils.GetDebugMessage("_imageQueue is found null"));

            lock (_imageQueue)
            {
                // Return if the task is stopped or a new task is started.
                if (taskId != this.TaskId) return;

                _imageQueue.Enqueue(image);
            }
        }

        /// <summary>
        /// It will pop the final Image at the beginning of the final image queue.
        /// </summary>
        /// <param name="taskId">
        /// Id of the task in which this function is called.
        /// </param>
        /// <returns>
        /// The final image to be displayed that is removed from the beginning.
        /// </returns>
        public Bitmap? GetFinalImage(int taskId)
        {
            Debug.Assert(_finalImageQueue != null, Utils.GetDebugMessage("_finalImageQueue is found null"));

            lock (_finalImageQueue)
            {
                // Check if the task is stopped or a new task is started.
                if (_finalImageQueue == null || taskId != this.TaskId)
                {
                    return null;
                }

                // Use a while loop to wait until the queue is not empty.
                while (_finalImageQueue.Count == 0)
                {
                    // Release the lock and wait for a signal.
                    Monitor.Wait(_finalImageQueue);

                    // After being woken up, recheck if the task is stopped or a new task is started.
                    if (_finalImageQueue == null || taskId != this.TaskId)
                    {
                        return null;
                    }
                }

                Debug.Assert(_finalImageQueue.Count > 0, Utils.GetDebugMessage("Queue should not be empty"));

                try
                {
                    return _finalImageQueue.Dequeue();
                }
                catch (InvalidOperationException e)
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Dequeue failed: {e.Message}", withTimeStamp: true));
                    return null;
                }
            }
        }

        /// <summary>
        /// Insert the final image into the final image queue.
        /// </summary>
        /// <param name="image">
        /// Image to be inserted.
        /// </param>
        /// /// <param name="taskId">
        /// Id of the task in which this function is called.
        /// </param>
        public void PutFinalImage(Bitmap image, int taskId)
        {
            Debug.Assert(_finalImageQueue != null, Utils.GetDebugMessage("_finalImageQueue is found null"));

            lock (_finalImageQueue)
            {
                // Return if the task is stopped or a new task is started.
                if (taskId != this.TaskId) return;

                _finalImageQueue.Enqueue(image);
            }
        }

        /// <summary>
        /// It will start image processing by calling the stitcher
        /// to process image and it will create (if not exist)
        /// and start the task for updating the displayed image.
        /// </summary>
        /// <param name="task">
        /// Task to be executed for updating current image of the client.
        /// </param>
        /// <exception cref="Exception"></exception>
        public void StartProcessing(Action<int> task)
        {
            Debug.Assert(_stitcher != null, Utils.GetDebugMessage("_stitcher is found null"));

            if (_imageSendTask != null)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Trying to start an already started task for the client with Id {this.Id}", withTimeStamp: true));
                return;
            }

            try
            {
                lock (_taskIdLock)
                {
                    // Create a new task Id
                    ++this.TaskId;

                    // Start the stitcher.
                    _stitcher?.StartStitching(this.TaskId);

                    // Create and start a new task.
                    _imageSendTask = new(() => task(this.TaskId));
                    _imageSendTask?.Start();
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to start the task: {e.Message}", withTimeStamp: true));
                throw new Exception("Failed to start the task", e);
            }

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully created the processing task with id {this.TaskId} for the client with id {this.Id}", withTimeStamp: true));
        }

        /// <summary>
        /// It will stop the image processing by stopping the stitcher.
        /// It will cancel the task for updating the displayed image
        /// and clear the queues containing images.
        /// </summary>
        /// <param name="stopAsync">
        /// To stop the process synchronously or not.
        /// </param>
        /// <exception cref="Exception"></exception>
        public void StopProcessing(bool stopAsync = false)
        {
            Debug.Assert(_stitcher != null, Utils.GetDebugMessage("_stitcher is null"));

            // Check if the task was started before.
            if (_imageSendTask == null)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Client with {this.Id} Id has never started their task", withTimeStamp: true));
                return;
            }

            // Store the previous image sending task.
            Task previousImageSendTask;

            try
            {
                lock (_taskIdLock)
                {
                    // Change the task ID to denote task cancellation.
                    ++this.TaskId;

                    // Immediately make the task variable null.
                    previousImageSendTask = _imageSendTask;
                    _imageSendTask = null;

                    // Clear both the queues.
                    lock (_imageQueue)
                    {
                        _imageQueue.Clear();
                    }

                    lock (_finalImageQueue)
                    {
                        _finalImageQueue.Clear();
                    }
                }

                if (!stopAsync)
                {
                    // Stop the stitcher and image sending task.
                    _stitcher?.StopStitching();
                    previousImageSendTask?.Wait();
                }
                else
                {
                    // Stop the stitcher and image sending task asynchronously.
                    Task.Run(() => _stitcher?.StopStitching());
                    Task.Run(() => previousImageSendTask?.Wait());
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to cancel the task: {e.Message}", withTimeStamp: true));
                throw new Exception("Failed to start the task", e);
            }

            Trace.WriteLine(Utils.GetDebugMessage($"Task with {this.TaskId} has stopped successfully for the client with id {this.Id}", withTimeStamp: true));
        }

        /// <summary>
        /// Resets the time of the timer object.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void UpdateTimer()
        {
            Debug.Assert(_timer != null, Utils.GetDebugMessage("_timer is found null"));

            try
            {
                // It will reset the timer to start again.
                _timer.Interval = SharedClientScreen.Timeout;
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to reset the timer: {e.Message}", withTimeStamp: true));
                throw new Exception("Failed to reset the timer", e);
            }
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
