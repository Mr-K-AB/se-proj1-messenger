using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace MessengerScreenshare.Client
{
    public class ScreenCapturer
    {
        private const int MaxQueueLength = 20;

        private CancellationTokenSource _cancellationTokenSource;
        private ConcurrentQueue<Bitmap> _capturedFrameQueue;
        private Screenshot _screenshot;

        public ScreenCapturer()
        {
            _capturedFrameQueue = new ConcurrentQueue<Bitmap>();
            _screenshot = Screenshot.Instance();
        }

        public async Task<Bitmap?> GetImageAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (_capturedFrameQueue.TryDequeue(out var frame))
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

        public int GetCapturedFrameLength()
        {
            return _capturedFrameQueue.Count;
        }

        public void StartCapture()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            Task.Run( async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_capturedFrameQueue.Count < MaxQueueLength)
                    {
                        try
                        {
                            Bitmap img = _screenshot.MakeScreenshot();
                            if (img != null)
                            {
                                await Task.Delay( 150 );
                                _capturedFrameQueue.Enqueue(img);
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine( $"[Screenshare] Could not capture screenshot: {e.Message}" );
                        }
                    }
                    else
                    {
                        // Sleep for some time if the queue is filled
                        while (_capturedFrameQueue.Count > MaxQueueLength / 2)
                        {
                            if (_capturedFrameQueue.TryDequeue( out var _ ))
                            {
                                await Task.Delay( 1 ); // Let's avoid busy waiting
                            }
                        }
                    }
                }
            } );
        }

        public void StopCapture()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
            }
            catch (Exception e)
            {
                Trace.WriteLine( Utils.GetDebugMessage( $"Unable to stop capture: {e.Message}" , withTimeStamp: true ) );
            }
        }
    }
}
