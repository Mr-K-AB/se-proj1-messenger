/******************************************************************************
* Filename    = ScreenCapturer.cs
*
* Author      = Alugonda Sathvik
*
* Product     = ScreenShare
* 
* Project     = Messenger
*
* Description = This Class captures and queues screen frames asynchronously, providing methods to start and stop the capture process.
*****************************************************************************/

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Controls;

namespace MessengerScreenshare.Client
{
    public class ScreenCapturer
    {
        /// <summary>
        /// The maximum allowed length of the captured frame queue.
        /// </summary>
        public const int MaxQueueLength = 20;

        private CancellationTokenSource? _cancellationTokenSource;
        private readonly ConcurrentQueue<Bitmap> _capturedFrameQueue;
        private readonly Screenshot _screenshot;

        /// <summary>
        /// Initializes a new instance of the ScreenCapturer class.
        /// </summary>
        public ScreenCapturer()
        {
            // Initialize the captured frame queue and obtain a singleton instance of Screenshot
            _capturedFrameQueue = new ConcurrentQueue<Bitmap>();
            _screenshot = Screenshot.Instance();
        }

        /// <summary>
        /// Asynchronously retrieves a captured screen frame from the queue.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to gracefully terminate the operation.</param>
        /// <returns>
        /// A Bitmap representing a screen frame if available in the queue; otherwise, null.
        /// </returns>
        public async Task<Bitmap?> GetImageAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (_capturedFrameQueue.TryDequeue(out Bitmap? frame))
                {
                    return frame;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }
                await Task.Delay(100);
            }
        }

        /// <summary>
        /// Gives Captured Frame Queue Size
        /// </summary>
        /// <returns>Captured Frame Queue Count </returns>
        public int GetCapturedFrameLength()
        {
            return _capturedFrameQueue.Count;
        }

        /// <summary>
        /// Initiates the screen capture process, continuously capturing and enqueueing screen frames.
        /// </summary>
        public void StartCapture()
        {
            // Initialize a new CancellationTokenSource for managing the capture task
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            // Start a new asynchronous task to perform screen capture
            Task.Run(async () =>
            {
                // Continue capturing frames until cancellation is requested
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Check if the captured frame queue is below the maximum allowed length
                    if (_capturedFrameQueue.Count < MaxQueueLength)
                    {
                        try
                        {
                            // Capture a screenshot and add it to the queue after a delay
                            Bitmap? img = _screenshot.MakeScreenshot();
                            if (img != null)
                            {
                                await Task.Delay(150);
                                _capturedFrameQueue.Enqueue(img);
                            }
                        }
                        catch (Exception e)
                        {
                            // Log an error message if capturing a screenshot fails
                            Trace.WriteLine($"[Screenshare] Could not capture screenshot: {e.Message}");
                        }
                    }
                    else
                    {
                        // Reduce the queue size by dequeuing frames if it exceeds half of the maximum length
                        while (_capturedFrameQueue.Count > MaxQueueLength / 2)
                        {
                            if (_capturedFrameQueue.TryDequeue(out Bitmap? _))
                            {
                                await Task.Delay(1); // Introduce a small delay to avoid busy waiting
                            }
                        }
                    }
                }
            });
        }


        /// <summary>
        /// Stops the screen capture process and clears the captured frame queue.
        /// </summary>
        public void StopCapture()
        {
            try
            {
                // Attempt to cancel the capture task using the CancellationTokenSource
                _cancellationTokenSource?.Cancel();
            }
            catch (Exception e)
            {
                // Log an error message if stopping capture encounters an exception
                Trace.WriteLine(Utils.GetDebugMessage($"Unable to stop capture: {e.Message}", withTimeStamp: true));
            }

            // Clear the captured frame queue to reset the capture process
            _capturedFrameQueue.Clear();
        }
    }
}
