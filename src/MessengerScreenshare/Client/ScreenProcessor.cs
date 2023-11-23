/******************************************************************************
* Filename    = ScreenProcessor.cs
*
* Author      = Alugonda Sathvik
*
* Product     = ScreenShare
* 
* Project     = Messenger
*
* Description = Thhis class processes captured screen frames asynchronously, compresses and queues them. 
*               It includes functionality for dynamic resolution adjustment and employs a XOR-based differencing approach to detect changes between consecutive frames.
*****************************************************************************/

using MessengerScreenshare.Client;
using MessengerScreenshare;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO.Compression;
using System.IO;

public class ScreenProcessor
{
    private const int MaxQueueLength = 20;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly ConcurrentQueue<string> _processedFrameQueue;
    private readonly ScreenCapturer _capturer;
    private Resolution _currentRes;
    private Resolution _newRes;
    public readonly object ResolutionLock;
    private int _capturedImageHeight;
    private int _capturedImageWidth;
    private readonly bool _cancellationToken;
    private Bitmap? _prevImage;
    private int _first_xor;

    /// <summary>
    /// Initializes a new instance of the ScreenProcessor class with a specified capturer.
    /// </summary>
    /// <param name="capturer">The ScreenCapturer instance for capturing screen frames.</param>
    public ScreenProcessor(ScreenCapturer capturer)
    {
        _capturer = capturer;
        _processedFrameQueue = new ConcurrentQueue<string>();
        ResolutionLock = new object();
        _first_xor = 0;
    }

    /// <summary>
    /// Asynchronously retrieves a processed screen frame from the queue.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to gracefully terminate the operation.</param>
    /// <returns>
    /// A string representing a processed screen frame if available in the queue; otherwise, an empty string.
    /// </returns>
    public async Task<string> GetFrameAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            // Try to dequeue a frame from the processed frame queue
            if (_processedFrameQueue.TryDequeue(out string? frame))
            {
                // Return the dequeued frame
                return frame;
            }

            // Check if cancellation is requested
            if (cancellationToken.IsCancellationRequested)
            {
                // Return an empty string to signal cancellation
                return "";
            }

            // Introduce a delay to avoid busy waiting and reduce resource consumption
            await Task.Delay(100);
        }
    }

    /// <summary>
    /// Gets the current length of the processed frame queue.
    /// </summary>
    /// <returns>The number of frames currently in the processed frame queue.</returns>
    public int GetProcessedFrameLength()
    {
        return _processedFrameQueue.Count;
    }

    /// <summary>
    /// Asynchronously starts the processing of captured screen frames.
    /// </summary>
    /// <param name="windowCount">The number of windows to divide the captured screen into.</param>
    public async Task StartProcessingAsync(int windowCount)
    {
        // Initialize a new CancellationTokenSource for managing the processing task
        _cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = _cancellationTokenSource.Token;

        // Initialize variables related to dynamic resolution adjustment
        _first_xor = 0;
        Bitmap? img = await _capturer.GetImageAsync(cancellationToken);
        _capturedImageHeight = img!.Height;
        _capturedImageWidth = img.Width;
        _newRes = new Resolution { Height = _capturedImageHeight / windowCount, Width = _capturedImageWidth / windowCount };
        _currentRes = _newRes;
        _prevImage = new Bitmap(_newRes.Width, _newRes.Height);

        while (!cancellationToken.IsCancellationRequested)
        {
            img = await _capturer.GetImageAsync(cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            // Compress and enqueue the processed frame
            string serializedBuffer = Compress(img!);
            if (_processedFrameQueue.Count < MaxQueueLength)
            {
                _processedFrameQueue.Enqueue(serializedBuffer);
            }
            else
            {
                // Reduce the queue size by dequeuing frames if it exceeds half of the maximum length
                while (_processedFrameQueue.Count > MaxQueueLength / 2)
                {
                    _processedFrameQueue.TryDequeue(out string? _);
                    await Task.Delay(1); // Avoid busy waiting
                }
            }

            _prevImage = img;
        }
    }

    /// <summary>
    /// Stops the processing of captured screen frames and clears the processed frame queue.
    /// </summary>
    public void StopProcessing()
    {
        try
        {
            // Attempt to cancel the processing task using the CancellationTokenSource
            _cancellationTokenSource?.Cancel();
        }
        catch (Exception e)
        {
            // Log an error message if stopping processing encounters an exception
            Trace.WriteLine(Utils.GetDebugMessage($"Failed to cancel processor task: {e.Message}", withTimeStamp: true));
        }

        // Clear the processed frame queue to reset the processing task
        _processedFrameQueue.Clear();
    }

    /// <summary>
    /// Sets a new resolution for processing frames based on the specified window count.
    /// </summary>
    /// <param name="windowCount">The number of windows to divide the captured screen into.</param>
    public void SetNewResolution(int windowCount)
    {
        Resolution res = new() { Height = _capturedImageHeight / windowCount, Width = _capturedImageWidth / windowCount };
        lock (ResolutionLock)
        {
            _newRes = res;
        }
    }

    /// <summary>
    /// Compresses a byte array using the Deflate algorithm.
    /// </summary>
    /// <param name="data">The byte array to be compressed.</param>
    /// <returns>The compressed byte array.</returns>
    public static byte[] CompressByteArray(byte[] data)
    {
        MemoryStream output = new();
        using (DeflateStream dstream = new(output, CompressionLevel.Fastest))
        {
            dstream.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }

    /// <summary>
    /// Compresses a Bitmap image using XOR-based differencing and Deflate compression.
    /// </summary>
    /// <param name="img">The Bitmap image to be compressed.</param>
    /// <returns>
    /// A string representing the compressed image data along with a flag indicating if it is a new image (0) or not (1).
    /// </returns>
    public string Compress(Bitmap img)
    {
        Bitmap? newImg;
        lock (ResolutionLock)
        {
            // Process the image if the previous image exists and the resolution remains the same
            if (_prevImage != null && _newRes == _currentRes)
            {
                newImg = Process(img, _prevImage);
            }
            else if (_newRes != _currentRes)
            {
                // Update the current resolution if it has changed
                _currentRes = _newRes;
            }
        }

        // Resize the image based on the current resolution
        img = new Bitmap(img, _currentRes.Width, _currentRes.Height);
        newImg = null;

        if (newImg == null)
        {
            // Compress and encode the image data, indicating it is a new image (flag = 1)
            MemoryStream ms = new();
            img.Save(ms, ImageFormat.Jpeg);
            byte[] data = CompressByteArray(ms.ToArray());
            _first_xor = 0;
            return Convert.ToBase64String(data) + "1";
        }
        else
        {
            // Compress and encode the image data, indicating it is not a new image (flag = 0)
            MemoryStream ms = new();
            newImg.Save(ms, ImageFormat.Bmp);
            byte[] data = CompressByteArray(ms.ToArray());
            return Convert.ToBase64String(data) + "0";
        }
    }

    /// <summary>
    /// Performs XOR-based differencing between two Bitmap images.
    /// </summary>
    /// <param name="curr">The current Bitmap image.</param>
    /// <param name="prev">The previous Bitmap image.</param>
    /// <returns>
    /// A new Bitmap image representing the XOR-based difference between the current and previous images,
    /// or null if the difference exceeds a threshold indicating significant changes.
    /// </returns>
    public static unsafe Bitmap? Process(Bitmap curr, Bitmap prev)
    {
        BitmapData currData = curr.LockBits(new Rectangle(0, 0, curr.Width, curr.Height), ImageLockMode.ReadWrite, curr.PixelFormat);
        BitmapData prevData = prev.LockBits(new Rectangle(0, 0, prev.Width, prev.Height), ImageLockMode.ReadWrite, prev.PixelFormat);

        int bytesPerPixel = Bitmap.GetPixelFormatSize(curr.PixelFormat) / 8;
        int heightInPixels = currData.Height;
        int widthInBytes = currData.Width * bytesPerPixel;

        byte* currPtr = (byte*)currData.Scan0;
        byte* prevPtr = (byte*)prevData.Scan0;

        Bitmap newBmp = new(curr.Width, curr.Height);
        BitmapData bmpData = newBmp.LockBits(new Rectangle(0, 0, curr.Width, curr.Height), ImageLockMode.ReadWrite, curr.PixelFormat);
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
                        // Threshold exceeded, significant changes detected, return null
                        curr.UnlockBits(currData);
                        prev.UnlockBits(prevData);
                        newBmp.UnlockBits(bmpData);
                        return null;
                    }
                }
            }
        }

        // Unlock the image data and return the new Bitmap representing the XOR-based difference
        curr.UnlockBits(currData);
        prev.UnlockBits(prevData);
        newBmp.UnlockBits(bmpData);
        return newBmp;
    }
}
