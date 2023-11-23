/******************************************************************************
* Filename    = Screenshot.cs
*
* Author      = Alugonda Sathvik
*
* Product     = ScreenShare
* 
* Project     = Messenger
*
* Description = This class implements the screenshot functionality.
* 
* Reference = https://github.com/0x2E757/ScreenCapturer , https://github.com/sharpdx/SharpDX-Samples/blob/master/Desktop/Direct3D11.1/ScreenCapture/Program.cs
*****************************************************************************/


using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Drawing;
using System.Drawing.Imaging;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Resource = SharpDX.DXGI.Resource;


namespace MessengerScreenshare.Client
{
    /// <summary>
    /// Class contains the necessary functions for taking the screenshot of the current screen.
    /// </summary>
    public class Screenshot
    {
        private static readonly object s_lock = new();
        private static Screenshot? s_instance;
        public bool CaptureActive { get; private set; }
        private Factory1? _factory1;
        private Adapter1? _adapter1;
        private Device? _device;
        private Output? _output;
        private Output1? _output1;
        private int _width;
        private int _height;
        private Rectangle _bounds;
        private Texture2DDescription _texture2DDescription;
        private Texture2D? _texture2D;
        private OutputDuplication? _outputDuplication;
        private Bitmap? _bitmap;

        private int _makeScreenshot_LastDisplayIndexValue;
        private int _makeScreenshot_LastAdapterIndexValue;

        protected Screenshot()
        {
            CaptureActive = false;
            InitializeVariables(0, 0, true);
        }

        public static Screenshot Instance()
        {
            lock (s_lock)
            {
                s_instance ??= new Screenshot();
                return s_instance;
            }
        }

        /// Core function of class for taking the screenshot. Uses SharpDX for faster image capture.
        /// </summary>
        /// <param name="displayIndex">Index for the display which is to be captured. Defaults to 0 (Primary Display)</param>
        /// <param name="adapterIndex">Index for the display card to be used. Defaults to 0 (Primary graphics card)</param>
        /// <param name="maxTimeout">Timeout to get duplicated frame</param>
        /// <returns>The bitmap image for the screenshot</returns>
        public Bitmap? MakeScreenshot(int displayIndex = 0, int adapterIndex = 0, int maxTimeout = 5000)
        {
            InitializeVariables( displayIndex , adapterIndex );

            // acquire the next frame and directly convert it into a bitmap
            if (_outputDuplication?.TryAcquireNextFrame(maxTimeout, out _, out Resource screenResource) != Result.Ok)
            {
                return null;
            }

            Texture2D screenTexture2D = screenResource.QueryInterface<Texture2D>();
            _device?.ImmediateContext.CopyResource(screenTexture2D, _texture2D);

            DataBox dataBox = _device!.ImmediateContext.MapSubresource(_texture2D, 0, MapMode.Read, MapFlags.None);

            Bitmap screenshot = new(_width, _height, PixelFormat.Format32bppRgb);
            BitmapData bitmapData = screenshot.LockBits(_bounds, ImageLockMode.WriteOnly, screenshot.PixelFormat);

            IntPtr dataBoxPointer = dataBox.DataPointer;
            IntPtr bitmapDataPointer = bitmapData.Scan0;

            for (int y = 0; y < _height; y++)
            {
                Utilities.CopyMemory(bitmapDataPointer, dataBoxPointer, _width * 4);
                dataBoxPointer = IntPtr.Add(dataBoxPointer, dataBox.RowPitch);
                bitmapDataPointer = IntPtr.Add(bitmapDataPointer, bitmapData.Stride);
            }

            screenshot.UnlockBits(bitmapData);
            _device.ImmediateContext.UnmapSubresource(_texture2D, 0);

            // release the acquired frame
            _outputDuplication.ReleaseFrame();
            return screenshot;
        }

        /// <summary>
        /// Initializes the members of the class.
        /// </summary>
        /// <param name="displayIndex">Index for the display which is to be captured. Defaults to 0 (Primary Display)</param>
        /// <param name="adapterIndex">Index for the display card to be used. Defaults to 0 (Primary graphics card)</param>
        /// <param name="forcedInitialization"></param>
        private void InitializeVariables(int displayIndex, int adapterIndex, bool forcedInitialization = false)
        {
            bool displayIndexChanged = _makeScreenshot_LastDisplayIndexValue != displayIndex;
            bool adapterIndexChanged = _makeScreenshot_LastAdapterIndexValue != adapterIndex;

            // reset all values in case of change in display, adapter or forced init.
            if (displayIndexChanged || adapterIndexChanged || forcedInitialization)
            {
                DisposeVariables();
                _factory1 = new Factory1();
                _adapter1 = _factory1.GetAdapter1(adapterIndex);
                _device = new Device(_adapter1);
                _output = _adapter1.GetOutput(displayIndex);
                _output1 = _output.QueryInterface<Output1>();
                _width = _output1.Description.DesktopBounds.Right - _output1.Description.DesktopBounds.Left;
                _height = _output1.Description.DesktopBounds.Bottom - _output1.Description.DesktopBounds.Top;
                _bounds = new Rectangle(Point.Empty, new Size(_width, _height));
                _texture2DDescription = new Texture2DDescription
                {
                    CpuAccessFlags = CpuAccessFlags.Read,
                    BindFlags = BindFlags.None,
                    Format = Format.B8G8R8A8_UNorm,
                    Width = _width,
                    Height = _height,
                    OptionFlags = ResourceOptionFlags.None,
                    MipLevels = 1,
                    ArraySize = 1,
                    SampleDescription = { Count = 1, Quality = 0 },
                    Usage = ResourceUsage.Staging
                };
                _texture2D = new Texture2D(_device, _texture2DDescription);
                _outputDuplication = _output1.DuplicateOutput(_device);
                _outputDuplication.TryAcquireNextFrame(1000, out _, out _);
                _outputDuplication.ReleaseFrame();
                _bitmap = new Bitmap(_width, _height, PixelFormat.Format32bppRgb);
                _makeScreenshot_LastAdapterIndexValue = adapterIndex;
                _makeScreenshot_LastDisplayIndexValue = displayIndex;
            }
        }

        /// <summary>
        /// Disposes the class memebers to avoid memory hogging.
        /// </summary>
        public void DisposeVariables()
        {
            _bitmap?.Dispose();
            _outputDuplication?.Dispose();
            _texture2D?.Dispose();
            _output1?.Dispose();
            _output?.Dispose();
            _device?.Dispose();
            _adapter1?.Dispose();
            _factory1?.Dispose();
            _makeScreenshot_LastAdapterIndexValue = -1;
            _makeScreenshot_LastDisplayIndexValue = -1;
        }
    }


}
