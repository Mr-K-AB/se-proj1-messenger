/******************************************************************************
 * 
 * Author      = A Sathvik
 *
 * Roll no     = 112001005
 *
 *****************************************************************************/
using MessengerScreenshare.Client;
using System.Drawing;
using System.Windows.Controls;

namespace MessengerTests.ScreenshareTests
{
    [TestClass]
    public class ScreenCapturerTests
    {
        /// <summary>
        /// Capture for some time and see if the captured images inside the queue are not null.
        /// </summary>
        [TestMethod]
        public void CorrectImageCaptureTest()
        {
            Task<ScreenCapturer> task = Task.Run(() => { ScreenCapturer screenCapturer = new(); return screenCapturer; });
            task.Wait();
            ScreenCapturer screenCapturer = task.Result;
            screenCapturer.StartCapture();
            Thread.Sleep(1000);

            int count = 0;
            CancellationTokenSource _cancellationTokenSource = new();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;
            _cancellationTokenSource.Dispose();
            for (int i = 0; i < 50; i++)
            {
                Task<Bitmap?> capturtask = screenCapturer.GetImageAsync(cancellationToken);
                capturtask.Wait();
                Bitmap? frame = capturtask.Result;

                if (frame != null)
                {
                    count++;
                }
            }

            screenCapturer.StopCapture();
            Assert.AreEqual(50, count);
        }

        /// <summary>
        /// Queue length should always be between 0 and MaxQueueLength.
        /// </summary>
        [TestMethod]
        public void CapturedFrameQueueFunctionality()
        {
            Task<ScreenCapturer> task = Task.Run(() => { ScreenCapturer screenCapturer = new(); return screenCapturer; });
            task.Wait();
            ScreenCapturer screenCapturer = task.Result;

            screenCapturer.StartCapture();
            Thread.Sleep(1000);
            int framesCaptured = screenCapturer.GetCapturedFrameLength();
            screenCapturer.StopCapture();

            Assert.IsTrue(framesCaptured is > 0 and <= ScreenCapturer.MaxQueueLength);
        }

        /// <summary>
        /// Test the control flow where screen ccapturing is stopped once and then restarted and stopped.
        /// This is to simulate a real-life scenario.
        /// </summary>
        [TestMethod]
        public void MultipleStartStopCapturer()
        {
            Task<ScreenCapturer> task = Task.Run(() => { ScreenCapturer screenCapturer = new(); return screenCapturer; });
            task.Wait();
            ScreenCapturer screenCapturer = task.Result;

            screenCapturer.StartCapture();
            Thread.Sleep(500);
            screenCapturer.StopCapture();
            Thread.Sleep(100);

            screenCapturer.StartCapture();
            Thread.Sleep(500);
            screenCapturer.StopCapture();

            // If the queue is empty, this means the capturing has been stopped sucessfully. 
            int framesCaptured = screenCapturer.GetCapturedFrameLength();
            Assert.AreEqual(0, framesCaptured);
        }
    }
}
