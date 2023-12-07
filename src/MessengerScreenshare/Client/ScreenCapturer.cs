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
using TraceLogger;

namespace MessengerScreenshare.Client
{
    public class ScreenCapturer
    {
        /// <summary>
        /// The maximum allowed length of the captured frame queue.
        /// </summary>
        public const int MaxQueueLength = 20;
        private int _clientnumber = 1;
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
                int serverLoad = GetServerLoad(); // You need to implement this method
                int dynamicDelay = CalculateDynamicDelay(GetCapturedFrameLength(), serverLoad, 100);

                await Task.Delay(dynamicDelay);
            }
        }

        /// <summary>
        /// Gives Captured Frame Queue Size
        /// </summary>
        /// <returns>Captured Frame Queue Count </returns>
        public int GetCapturedFrameLength() {
            return _capturedFrameQueue.Count;
        }

        /// <summary>
        /// Initiates the screen capture process, continuously capturing and enqueueing screen frames.
        /// </summary>
        public void StartCapture()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    int currentQueueLength = GetCapturedFrameLength();

                    // Calculate dynamic delay based on the number of connected clients or server load
                    int serverLoad = GetServerLoad(); // You need to implement this method
                    int dynamicDelay = CalculateDynamicDelay(currentQueueLength, serverLoad, 150);

                    try
                    {
                        Bitmap? img = _screenshot.MakeScreenshot();
                        if (img != null)
                        {
                            await Task.Delay(dynamicDelay);
                            _capturedFrameQueue.Enqueue(img);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log($"[Screenshare] Could not capture screenshot: {e.Message}", LogLevel.ERROR);
                    }
                }
            });
        }

        // Calculate dynamic delay based on the current queue length and server load
        private int CalculateDynamicDelay(int currentQueueLength, int serverLoad, int maxDelay)
        {
            // Adjust the multiplier based on the server load
            int dynamicDelay = currentQueueLength * serverLoad;

            // Ensure that the dynamic delay does not exceed the maximum allowed delay
            dynamicDelay = Math.Min(dynamicDelay, maxDelay);

            return dynamicDelay;
        }

        // Get an estimate of server load (you need to implement this based on your server logic)
        private int GetServerLoad()
        {
            return _clientnumber; // Assuming ConnectedClients is a collection of connected clients
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
            { // Log an error message if stopping capture encounters an exception
                Logger.Log($"Unable to stop capture: {e.Message}", LogLevel.ERROR);
            }

            // Clear the captured frame queue to reset the capture process
            _capturedFrameQueue.Clear();
        }

        public void SetNoofClients(int noofClients)
        {
            _clientnumber = noofClients;
        }
    }
}
