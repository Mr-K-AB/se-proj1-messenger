/******************************************************************************
 * Filename    = ScreenStitcher.cs
 *
 * Author      = Aditya Raj
 *
 * Product     = Messenger
 * 
 * Project     = MessengerScreenshare
 *
 * Description = Screen stitching functionality is implemented in this file. 
                 This file contains the ScreenStitcher class and used by 
                 ScreenshareServer.
 *****************************************************************************/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

namespace MessengerScreenshare.Server
{
    public class ScreenStitcher
    {
        /// <summary>
        /// SharedClientScreen object.
        /// </summary>
        private readonly SharedClientScreen _sharedClientScreen;

        /// <summary>
        /// Thread to run stitcher.
        /// </summary>
        private Task? _stitchTask;

        /// <summary>
        /// A private variable to store prior image.
        /// </summary>
        private Bitmap? _priorImage;

        /// <summary>
        /// Old resolution of the image.
        /// </summary>
        private Resolution? _resolution;

        /// <summary>
        /// A count to maintain the number of image stitched. Used in
        /// trace logs.
        /// </summary>
        private int _cnt = 0;

        /// <summary>
        /// Constructor for ScreenSticher.
        /// </summary>
        /// <param name="_sharedClientScreen"></param>
        public ScreenStitcher(SharedClientScreen _sharedClientScreen)
        {
            _priorImage = null;
            _stitchTask = null;
            _resolution = null;
            this._sharedClientScreen = _sharedClientScreen;
        }

        /// <summary>
        /// Client may sends only a 'diff' instead of full image to server.
        /// So to retrieve full image using diff, this function is called.
        /// </summary>
        /// <param name="curr">
        /// The 'diff' current image
        /// </param>
        /// <param name="prev">
        /// Previous image
        /// </param>
        /// <returns>
        /// Current image meant to be displayed
        /// </returns>
        public static unsafe Bitmap Process(Bitmap curr, Bitmap prev)
        {
            BitmapData currData = curr.LockBits(new Rectangle(0, 0, curr.Width, curr.Height), ImageLockMode.ReadWrite, curr.PixelFormat);
            BitmapData prevData = prev.LockBits(new Rectangle(0, 0, prev.Width, prev.Height), ImageLockMode.ReadWrite, prev.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(curr.PixelFormat) / 8;
            int heightInPixels = currData.Height;
            int widthInBytes = currData.Width * bytesPerPixel;

            byte* currptr = (byte*)currData.Scan0;
            byte* prevptr = (byte*)prevData.Scan0;

            Bitmap newb = new(curr.Width, curr.Height);
            BitmapData bmd = newb.LockBits(new Rectangle(0, 0, 10, 10), System.Drawing.Imaging.ImageLockMode.ReadOnly, newb.PixelFormat);
            byte* ptr = (byte*)bmd.Scan0;

            for (int y = 0; y < heightInPixels; y++)
            {
                int currentLine = y * currData.Stride;

                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    int oldBlue = currptr[currentLine + x];
                    int oldGreen = currptr[currentLine + x + 1];
                    int oldRed = currptr[currentLine + x + 2];
                    int oldAlpha = currptr[currentLine + x + 3];

                    int newBlue = prevptr[currentLine + x];
                    int newGreen = prevptr[currentLine + x + 1];
                    int newRed = prevptr[currentLine + x + 2];
                    int newAlpha = prevptr[currentLine + x + 3];

                    ptr[currentLine + x] = (byte)(oldBlue ^ newBlue);
                    ptr[currentLine + x + 1] = (byte)(oldGreen ^ newGreen);
                    ptr[currentLine + x + 2] = (byte)(oldRed ^ newRed);
                    ptr[currentLine + x + 3] = (byte)(oldAlpha ^ newAlpha);
                }
            }

            curr.UnlockBits(currData);
            prev.UnlockBits(prevData);
            newb.UnlockBits(bmd);

            return newb;
        }

        /// <summary>
        /// Method to decompress a byte array compressed by processor.
        /// </summary>
        /// <param name="data">
        /// Byte array compressed using GZipStream
        /// </param>
        /// <returns>
        /// Decompressed byte array
        /// </returns>
        public static byte[] DecompressByteArray(byte[] data)
        {
            using MemoryStream input = new(data);
            using MemoryStream output = new();
            using (DeflateStream dstream = new(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

        /// <summary>
        /// It will create the task if not exist and start it.
        /// It will read the image using `_sharedClientScreen.GetFrame`
        /// and insert into the final image using `_sharedClientScreen.PutFinalImage`
        /// </summary>
        /// <param name="taskId">
        /// Id of the task in which this function is called.
        /// </param>
        public void StartStitching(int taskId)
        {
            if (_stitchTask != null)
            {
                return;
            }

            _stitchTask = new Task(() =>
            {
                while (taskId == _sharedClientScreen.TaskId)
                {
                    string? newFrame = _sharedClientScreen.GetImage(taskId);

                    if (taskId != _sharedClientScreen.TaskId)
                    {
                        break;
                    }
                    if (newFrame == null)
                    {
                        Trace.WriteLine(Utils.GetDebugMessage("New frame returned by _sharedClientScreen is null.", withTimeStamp: true));
                        continue;
                    }

                    Bitmap stichedImage = Stitch(_priorImage, newFrame);
                    Trace.WriteLine(Utils.GetDebugMessage($"STITCHED image from client {_cnt++}", withTimeStamp: true));
                    _priorImage = stichedImage;
                    _sharedClientScreen.PutFinalImage(stichedImage, taskId);
                }
            });

            _stitchTask?.Start();

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully created the stitching task with id {taskId} for the client with id {_sharedClientScreen.Id}", withTimeStamp: true));
        }

        /// <summary>
        /// Method to stop the stitcher task.
        /// </summary>
        public void StopStitching()
        {
            if (_stitchTask == null)
            {
                return;
            }

            Task previousStitchTask = _stitchTask;
            _stitchTask = null;

            try
            {
                previousStitchTask?.Wait();
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to start the stitching: {e.Message}", withTimeStamp: true));
            }

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully stopped the processing task for the client with id {_sharedClientScreen.Id}", withTimeStamp: true));

        }

        /// <summary>
        /// Stitch new frame over prior image is implemented. Data received
        /// from client has '1' in front then new image has arrived hence 
        /// the stitching will not happen. And if it is '0' then client has
        /// sent the 'diff' part only so stitching will happen (basically 
        /// calls the process function) and find out the current image.
        /// </summary>
        /// <param name="priorImage"></param>
        /// <param name="newFrame"></param>
        /// <returns>
        /// New image after stitching
        /// </returns>
        public Bitmap Stitch(Bitmap? priorImage, string newFrame)
        {
            char isCompleteFrame = newFrame[newFrame.Length - 1];

            newFrame = newFrame.Remove(newFrame.Length - 1);

            byte[]? frameData = Convert.FromBase64String(newFrame);
            frameData = DecompressByteArray(frameData);

            MemoryStream frameStream = new(frameData);
            Bitmap xorBitmap = new(frameStream);
            {
                var newResolution = new Resolution() { Height = xorBitmap.Height, Width = xorBitmap.Width };

                if (priorImage == null || newResolution != _resolution)
                {
                    priorImage = new Bitmap(newResolution.Width, newResolution.Height);
                }

                if (isCompleteFrame == '1')
                {
                    priorImage = xorBitmap;
                }
                else
                {
                    priorImage = Process(xorBitmap, priorImage);
                }

                _resolution = newResolution;
            }

            return priorImage;
        }
    }
}
