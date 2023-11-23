/******************************************************************************
 * 
 * Author      = A Sathvik
 *
 * Roll no     = 112001005
 *
 *****************************************************************************/
using MessengerScreenshare.Client;
using MessengerScreenshare.Server;
using MessengerScreenshare;
using System.Drawing;
using System.Reflection;


namespace MessengerTests.ScreenshareTests
{
    [TestClass]
    public class ScreenProcessorTests
    {
        [TestMethod]
        public void TestProcessedFrameNonEmpty()
        {
            ScreenCapturer screenCapturer = new();
            ScreenProcessor screenProcessor = new(screenCapturer);

            // Capturer must be called before Processor
            screenCapturer.StartCapture();
            Task.Run(() => screenProcessor.StartProcessingAsync(1));

            Thread.Sleep(1000);

            screenCapturer.StopCapture();
            int v2 = screenProcessor.GetProcessedFrameLength();
            screenProcessor.StopProcessing();

            Assert.IsTrue(v2 > 0);
        }

        [TestMethod]
        public void TestCleanup()
        {
            ScreenCapturer screenCapturer = new();
            ScreenProcessor screenProcessor = new(screenCapturer);

            screenCapturer.StartCapture();
            Task.Run(() => screenProcessor.StartProcessingAsync(1));

            Thread.Sleep(1000);

            screenCapturer.StopCapture();
            screenProcessor.StopProcessing();

            Console.WriteLine($"len = {screenProcessor.GetProcessedFrameLength()}");
            Assert.IsTrue(screenProcessor.GetProcessedFrameLength() == 0);
        }

        [TestMethod]
        public void TestSameImagePixelDiffZero()
        {
            ScreenCapturer screenCapturer = new();

            CancellationTokenSource _cancellationTokenSource = new();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;
            _cancellationTokenSource.Dispose();

            screenCapturer.StartCapture();
            Bitmap? img = Task.Run(() => screenCapturer.GetImageAsync(cancellationToken)).Result;
            screenCapturer.StopCapture();
            Assert.IsTrue(img != null);

            Bitmap emptyImage = new(img.Width, img.Height);
            Bitmap? tmp = ScreenStitcher.Process(img, emptyImage);
            Assert.IsTrue(tmp != null);
            Assert.IsTrue(Utils.CompareBitmap(tmp, img));
        }

        [TestMethod]
        public void TestResolutionChange()
        {
            ScreenCapturer screenCapturer = new();
            ScreenProcessor screenProcessor = new(screenCapturer);

            screenCapturer.StartCapture();
            Task.Run(() => screenProcessor.StartProcessingAsync(1));

            Thread.Sleep(1000);

            Resolution? res1 = (Resolution?)typeof(ScreenProcessor)
                .GetField("_currentRes", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(screenProcessor);

            Assert.IsNotNull(res1);

            screenProcessor.SetNewResolution(9);

            Thread.Sleep(1000);

            Resolution? res2 = (Resolution?)typeof(ScreenProcessor)
                .GetField("_currentRes", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(screenProcessor);

            Assert.IsNotNull(res2);

            screenCapturer.StopCapture();
            screenProcessor.StopProcessing();

            Assert.IsTrue(res1?.Height / 9 == res2?.Height);
            Assert.IsTrue(res1?.Width / 9 == res2?.Width);
        }

        [TestMethod]
        public void TestCorrectImageStringFormat()
        {
            ScreenCapturer screenCapturer = new();
            ScreenProcessor screenProcessor = new(screenCapturer);

            screenCapturer.StartCapture();
            Task.Run(() => screenProcessor.StartProcessingAsync(1));

            int cnt = 5;
            while (cnt-- > 0)
            {
                bool token = false;
                CancellationTokenSource _cancellationTokenSource = new();
                CancellationToken cancellationToken = _cancellationTokenSource.Token;
                _cancellationTokenSource.Dispose();
                string tmpStr = screenProcessor.GetFrameAsync(cancellationToken).Result;
                Assert.IsTrue(tmpStr[^1] == '0' || tmpStr[^1] == '1');
            }

            screenCapturer.StopCapture();
            screenProcessor.StopProcessing();
        }
    }
}

// calling processing again should give the image back
// stop should make the queue length change stop
// finally it must have last character as 0 or 1
