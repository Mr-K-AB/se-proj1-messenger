using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace MessengerScreenshare.Client
{
    public class ScreenProcessor
    {
        private const int MaxQueueLength = 20;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ConcurrentQueue<string> _processedFrameQueue;
        private readonly ScreenCapturer _capturer;
        private Resolution _currentRes;
        private Resolution _newRes;
        public readonly object ResolutionLock;
        private int _capturedImageHeight;
        private int _capturedImageWidth;
        private readonly bool _cancellationToken;
        private Bitmap _prevImage;
        private int _first_xor;

        public ScreenProcessor( ScreenCapturer capturer )
        {
            _capturer = capturer;
            _processedFrameQueue = new ConcurrentQueue<string>();
            ResolutionLock = new object();
            _first_xor = 0;
        }

        public async Task<string> GetFrameAsync( CancellationToken cancellationToken )
        {
            while (true)
            {
                if (_processedFrameQueue.TryDequeue( out string? frame ))
                {
                    return frame;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return "";
                }

                await Task.Delay( 100 );
            }
        }

        public int GetProcessedFrameLength()
        {
            return _processedFrameQueue.Count;
        }

        public async Task StartProcessingAsync( int windowCount )
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            _first_xor = 0;
            Bitmap img = await _capturer.GetImageAsync( cancellationToken );
            _capturedImageHeight = img.Height;
            _capturedImageWidth = img.Width;
            _newRes = new Resolution { Height = _capturedImageHeight / windowCount , Width = _capturedImageWidth / windowCount };
            _currentRes = _newRes;
            _prevImage = new Bitmap( _newRes.Width , _newRes.Height );

            while (!cancellationToken.IsCancellationRequested)
            {
                Bitmap img = await _capturer.GetImageAsync( cancellationToken );
                if (cancellationToken.IsCancellationRequested)
                    break;

                string serializedBuffer = Compress( img );

                if (_processedFrameQueue.Count < MaxQueueLength)
                {
                    _processedFrameQueue.Enqueue( serializedBuffer );
                }
                else
                {
                    while (_processedFrameQueue.Count > MaxQueueLength / 2)
                    {
                        _processedFrameQueue.TryDequeue( out var _ );
                        await Task.Delay( 1 ); // Avoid busy waiting
                    }
                }

                _prevImage = img;
            }
        }

        public void StopProcessing()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
            }
            catch (Exception e)
            {
                Trace.WriteLine( Utils.GetDebugMessage( $"Failed to cancel processor task: {e.Message}" , withTimeStamp: true ) );
            }

            _processedFrameQueue.Clear();
        }

        public void SetNewResolution( int windowCount )
        {
            Resolution res = new Resolution { Height = _capturedImageHeight / windowCount , Width = _capturedImageWidth / windowCount };
            lock (ResolutionLock)
            {
                _newRes = res;
            }
        }

        public static byte[] CompressByteArray( byte[] data )
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream dstream = new DeflateStream( output , CompressionLevel.Fastest ))
                {
                    dstream.Write( data , 0 , data.Length );
                }
                return output.ToArray();
            }
        }

        public string Compress( Bitmap img )
        {
            Bitmap newImg = null;

            lock (ResolutionLock)
            {
                if (_prevImage != null && _newRes == _currentRes)
                {
                    newImg = Process( img , _prevImage );
                }
                else if (_newRes != _currentRes)
                {
                    _currentRes = _newRes;
                }
            }

            img = new Bitmap( img , _currentRes.Width , _currentRes.Height );
            newImg = null;

            if (newImg == null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save( ms , ImageFormat.Jpeg );
                    var data = CompressByteArray( ms.ToArray() );
                    _first_xor = 0;
                    return Convert.ToBase64String( data ) + "1";
                }
            }
            else
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    newImg.Save( ms , ImageFormat.Bmp );
                    var data = CompressByteArray( ms.ToArray() );
                    return Convert.ToBase64String( data ) + "0";
                }
            }
        }

        public static unsafe Bitmap? Process( Bitmap curr , Bitmap prev )
        {
            BitmapData currData = curr.LockBits( new Rectangle( 0 , 0 , curr.Width , curr.Height ) , ImageLockMode.ReadWrite , curr.PixelFormat );
            BitmapData prevData = prev.LockBits( new Rectangle( 0 , 0 , prev.Width , prev.Height ) , ImageLockMode.ReadWrite , prev.PixelFormat );

            int bytesPerPixel = Bitmap.GetPixelFormatSize( curr.PixelFormat ) / 8;
            int heightInPixels = currData.Height;
            int widthInBytes = currData.Width * bytesPerPixel;

            byte* currPtr = (byte*)currData.Scan0;
            byte* prevPtr = (byte*)prevData.Scan0;

            Bitmap newBmp = new Bitmap( curr.Width , curr.Height );
            BitmapData bmpData = newBmp.LockBits( new Rectangle( 0 , 0 , curr.Width , curr.Height ) , ImageLockMode.ReadWrite , curr.PixelFormat );
            byte* ptr = (byte*)bmpData.Scan0;

            int diff = 0;

            for (int y = 0; y < heightInPixels; y++)
            {
                int currentLine = y * currData.Stride;

                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    int oldBlue = currPtr[currentLine + x];
                    int oldGreen = currPtr[currentLine + x + 1];
                    int oldRed = currPtr[currentLine + x + 2];
                    int oldAlpha = currPtr[currentLine + x + 3];

                    int newBlue = prevPtr[currentLine + x];
                    int newGreen = prevPtr[currentLine + x + 1];
                    int newRed = prevPtr[currentLine + x + 2];
                    int newAlpha = prevPtr[currentLine + x + 3];

                    ptr[currentLine + x] = (byte)(oldBlue ^ newBlue);
                    ptr[currentLine + x + 1] = (byte)(oldGreen ^ newGreen);
                    ptr[currentLine + x + 2] = (byte)(oldRed ^ newRed);
                    ptr[currentLine + x + 3] = (byte)(oldAlpha ^ newAlpha);

                    if ((oldBlue != newBlue) || (oldGreen != newGreen) || (oldRed != newRed) || (oldAlpha != newAlpha))
                    {
                        diff++;
                        if (diff > 500)
                        {
                            curr.UnlockBits( currData );
                            prev.UnlockBits( prevData );
                            newBmp.UnlockBits( bmpData );
                            return null;
                        }
                    }
                }
            }

            curr.UnlockBits(currData);
            prev.UnlockBits( prevData);
            newBmp.UnlockBits(bmpData);
           
            return newBmp;
        }
    }
}
