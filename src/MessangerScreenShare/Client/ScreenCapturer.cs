///<author>Alugonda Sathvik</author>
///<summary> 
///This file contains the ScreenCapturer Class that implements the
///screen capturing functionality. It is used by ScreenshareClient. 
///</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MessangerScreenShare.Client
{
    /// <summary>
    /// This class is considered as helper class for ScreenCapturer
    /// where it is used to store image data as bytes.
    /// </summary>
    public class CapturedScreen
    {
        public byte[] ImageData { get; }

        public CapturedScreen( byte[] imageData )
        {
            ImageData = imageData;
        }
    }

    /// <summary>
    /// This class is used to capture the screen 
    /// with respect to client view i.e can able to 
    /// start capturing screen and stop capturing screen.
    /// </summary>

    public class ScreenCapturer
    {
        private readonly Queue<CapturedScreen> _capturedFrames;

        // Limits the number of frames in the queue
        public const int MaxQueueLength = 50;

        private bool _cancellationToken;

        // _captureTask variable is used to 
        // manage start the capturing screen task
        private Task? _captureTask;

        private readonly Screenshot _screenshot;

        public ScreenCapturer()
        {
            _capturedFrames = new Queue<CapturedScreen>();
            _screenshot = Screenshot.Instance();
        }

        /// <summary>
        /// GetCapturedScreen gives the captured Screen 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns> The first captured screen in the _capturedFrames queue.</returns>
        public CapturedScreen? GetCapturedScreen( ref bool cancellationToken )
        {
            while (true)
            {
                lock (_capturedFrames)
                {
                    if (_capturedFrames.Count != 0)
                    {
                        break;
                    }
                }

                if (cancellationToken)
                {
                    return null;
                }

                Thread.Sleep( 100 );
            }

            lock (_capturedFrames)
            {
                return _capturedFrames?.Dequeue();
            }
        }

        public int GetCapturedFrameLength()
        {
            lock (_capturedFrames)
            {
                return _capturedFrames.Count;
            }
        }

        public void StartCapture()
        {
            _cancellationToken = false;
            _captureTask = new Task( () =>
            {
                while (!_cancellationToken)
                {
                    lock (_capturedFrames)
                    {
                        if (_capturedFrames.Count < MaxQueueLength)
                        {
                            try
                            {
                                byte[] imageData = _screenshot.MakeScreenShot();
                                if (imageData != null)
                                {
                                    Thread.Sleep( 150 );
                                    _capturedFrames.Enqueue( new CapturedScreen( imageData ) );
                                }
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine( $"[Screenshare] Could not capture screenshot: {e.Message}" );
                            }
                        }
                        else
                        {
                            // Sleep for some time, if the queue is filled
                            while (_capturedFrames.Count > MaxQueueLength / 2)
                            {
                                _capturedFrames.Dequeue();
                            }
                        }
                    }
                }
            } );

            _captureTask.Start();
        }

        public void StopCapture()
        {
            try
            {
                _cancellationToken = true;
                _captureTask?.Wait();
            }
            catch (Exception e)
            {
                Trace.WriteLine( Utils.GetDebugMessage( $"Unable to stop capture: {e.Message}" , withTimeStamp: true ) );
            }

            _capturedFrames.Clear();
        }
    }
}
