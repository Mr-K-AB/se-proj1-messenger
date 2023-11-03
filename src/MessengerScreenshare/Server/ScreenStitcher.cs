///<author>Aditya Raj</author>
///<summary>
/// Screen stitching functionality is implemented in this file. 
///This file contains the ScreenStitcher class and used by ScreenshareServer.
///</summary>


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
        public ScreenStitcher( SharedClientScreen _sharedClientScreen )
        {
            this._priorImage = null;
            this._stitchTask = null;
            this._resolution = null;
            this._sharedClientScreen = _sharedClientScreen;
        }

        /// <summary>
        /// Client may sends only a 'diff' instead of full image to server.
        /// So to retrieve full image using diff, this function is called.
        /// </summary>
        /// <param name="curr">The 'diff' current image</param>
        /// <param name="prev">Previous image</param>
        /// <returns>Current image meant to be displayed</returns>
        public static unsafe Bitmap Process(Bitmap curr, Bitmap prev)
        {
            BitmapData currData = curr.LockBits(new Rectangle(0, 0, curr.Width, curr.Height), ImageLockMode.ReadOnly, curr.PixelFormat);
            BitmapData prevData = prev.LockBits(new Rectangle(0, 0, prev.Width, prev.Height), ImageLockMode.ReadOnly, prev.PixelFormat);

            int bytesPerPixel = Image.GetPixelFormatSize(curr.PixelFormat) / 8;
            int width = currData.Width;
            int height = currData.Height;

            Bitmap newBitmap = new Bitmap(width, height, curr.PixelFormat);
            BitmapData newData = newBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, curr.PixelFormat);

            byte* currPtr = (byte*)currData.Scan0;
            byte* prevPtr = (byte*)prevData.Scan0;
            byte* newPtr = (byte*)newData.Scan0;

            for (int y = 0; y < height; y++)
            {
                int offset = y * currData.Stride;

                for (int x = 0; x < width * bytesPerPixel; x++)
                {
                    byte currByte = currPtr[offset + x];
                    byte prevByte = prevPtr[offset + x];

                    newPtr[offset + x] = (byte)(currByte ^ prevByte);
                }
            }

            curr.UnlockBits(currData);
            prev.UnlockBits(prevData);
            newBitmap.UnlockBits(newData);

            return newBitmap;
        }

        /// <summary>
        /// Method to decompress a byte array compressed by processor.
        /// </summary>
        /// <param name="data">Byte array compressed using GZipStream</param>
        /// <returns>Decompressed byte array</returns>
        public static byte[] DecompressByteArray( byte[] data )
        {
            using (MemoryStream input = new MemoryStream( data ))
            using (MemoryStream output = new MemoryStream())
            using (GZipStream gzipStream = new GZipStream( input , CompressionMode.Decompress ))
            {
                gzipStream.CopyTo( output );
                return output.ToArray();
            }
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

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            _stitchTask = Task.Run(() =>
            {
                CancellationToken token = cancellationTokenSource.Token;

                Trace.WriteLine(Utils.GetDebugMessage($"Successfully created the stitching task with id {taskId} for the client with id {_sharedClientScreen.Id}", withTimeStamp: true));

                while (!token.IsCancellationRequested)
                {
                    string newFrame = _sharedClientScreen.GetImage(taskId);

                    if (newFrame == null)
                    {
                        Trace.WriteLine(Utils.GetDebugMessage("New frame returned by _sharedClientScreen is null.", withTimeStamp: true));
                        continue;
                    }

                    Bitmap stitchedImage = Stitch(_priorImage, newFrame);
                    Trace.WriteLine(Utils.GetDebugMessage($"STITCHED image from client {_cnt++}", withTimeStamp: true));
                    _oldImage = stitchedImage;
                    _sharedClientScreen.PutFinalImage(stitchedImage, taskId);
                }
            }, cancellationTokenSource.Token);

            _stitchTask.Start();
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

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            cancellationTokenSource.Cancel();

            try
            {
                _stitchTask.Wait();
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to start the stitching: {e.Message}", withTimeStamp: true));
            }

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully stopped the processing task for the client with id {_sharedClientScreen.Id}", withTimeStamp: true));

        }
    }

    /// <summary>
    /// Stitch new frame over prior image is implemented. Data received
    /// from client has '1' in front then new image has arrived hence 
    /// the stitching will not happen. And if it is '0' then client has
    /// sent the 'diff' part only so stitching will happen (basically 
    /// calls the process function) and find out the current image.
    /// </summary>
    /// <param name="oldImage"></param>
    /// <param name="newFrame"></param>
    /// <returns>New image after stitching</returns>
    private Bitmap Stitch(Bitmap oldImage, string newFrame)
    {
        char isCompleteFrame = newFrame[newFrame.Length - 1];
        newFrame = newFrame.Remove(newFrame.Length - 1);

        byte[] frameData = Convert.FromBase64String(newFrame);
        frameData = DecompressByteArray(frameData);

        using (MemoryStream frameStream = new MemoryStream(frameData))
        using (Bitmap xorBitmap = new Bitmap(frameStream))
        {
            var newResolution = new Resolution() {Height = xorBitmap.Height, Width = xorBitmap.Width};

            if (oldImage == null || newResolution != _resolution)
            {
                oldImage?.Dispose();  
                oldImage = new Bitmap(newResolution.Width , newResolution.Height);
            }

            if (isCompleteFrame == '1')
            {
                oldImage.Dispose();  
                oldImage = xorBitmap;
            }
            else
            {
                oldImage = Process(xorBitmap, oldImage);
            }

            _resolution = newResolution;
        }

        return oldImage;
    }


}
