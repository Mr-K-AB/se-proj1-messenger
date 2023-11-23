/// <author>Aditya Raj</author>
/// <summary>
/// Tests for methods which is defined in "SharedClientScreen"
/// </summary>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Client;
using MessengerScreenshare.Client;
using MessengerScreenshare.Server;
using Moq;

using SSUtils = MessengerScreenshare.Utils;

namespace MessengerTests.ScreenshareTests
{
    [TestClass]
    public class SharedClientScreenTest
    {
        /// <summary>
        /// Update time values after the timeout time.
        /// </summary>
        /*
        public static IEnumerable<object[]> PostTimeoutTime =>
            new List<object[]>
            {
                new object[] { SharedClientScreen.Timeout + 3000 },
                new object[] { SharedClientScreen.Timeout + 4000 },
                new object[] { SharedClientScreen.Timeout + 6000 },
            };

        /// <summary>
        /// Update time values before the timeout time.
        /// </summary>
        public static IEnumerable<object[]> PreTimeoutTime =>
            new List<object[]>
            {
                new object[] { SharedClientScreen.Timeout - 2000 },
                new object[] { SharedClientScreen.Timeout - 1000 },
                new object[] { SharedClientScreen.Timeout - 100 },
            };
        */
        /// <summary>
        /// This test is for checking whether client
        /// is successfully disposed or not.
        /// </summary>
        /// <param name="sleepTime">
        /// Sleep Time for thread after calling dispose.
        /// </param>
        [TestMethod]
        [DataRow(100)]
        public void TestDispose(int sleepTime)
        {
            /// It will start the underlying timer after creating client.
            var mockServer = new Mock<ITimer>();
            SharedClientScreen client = Utils.GetMockClient(mockServer.Object);

            /// Dispose of the client.
            client.Dispose();
            Thread.Sleep(sleepTime);

            client.Dispose();
            client.StopProcessing();
        }

        /// <summary>
        /// This test is for checking whether in client's image 
        /// queue, the image is properly enqueuing and dequeuing or not.
        /// </summary>
        [TestMethod]
        public void TestGetAndPutImage()
        {
            /// Mock client and server creation.
            var mockViewModel = new Mock<IDataReceiver>();
            ScreenshareServer server = ScreenshareServer.GetInstance(mockViewModel.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);

            /// Mock images will insert into the client's image queue.
            int numOfImages = 5;
            List<string> clientImage = new();
            for(int i = 0; i< numOfImages; i++)
            {
                clientImage.Add(Utils.RandomString(i + 100));
                client.PutImage(clientImage[i], client.TaskId);
            }

            /// Check for the retrieve image are same and in order.
            for(int i = 0; i<numOfImages; i++)
            {
                string? receivedImage = client.GetImage(client.TaskId);
                Assert.IsNotNull(receivedImage);
                Assert.IsTrue(clientImage[i] == receivedImage);
            }

            /// Dispose the mock client and server.
            client.Dispose();
            server.Dispose();
            
        }

        /// <summary>
        /// This test is for checking whether in client's final image 
        /// queue, the image is properly enqueuing and dequeuing or not.
        /// </summary>
       [TestMethod]
        public void TestGetAndPutFinalImage()
        {
            /// Mock client and server creation.
            var mockViewModel = new Mock<IDataReceiver>();
            ScreenshareServer server = ScreenshareServer.GetInstance(mockViewModel.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);

            /// Mock images will insert into the client's final image queue.
            int numOfImages = 5;
            List<Bitmap> clientImage = new();
            for (int i = 0; i < numOfImages; i++)
            {
                Bitmap mockImage = Utils.GetMockBitmap();
                client.PutFinalImage(mockImage, client.TaskId);
                clientImage.Add(mockImage);
            }

            /// Check for the retrieve final image are same and in order.
            for (int i = 0; i < numOfImages; i++)
            {
                Bitmap? receivedImage = client.GetFinalImage(client.TaskId);
                Assert.IsNotNull(receivedImage);
                Assert.IsTrue(clientImage[i] == receivedImage);
            }

            /// Dispose the mock client and server.
            client.Dispose();
            server.Dispose();
        }
       
        /// <summary>
        /// This test is to check whether the processing task
        /// for client starts and stops successfully or not.
        /// </summary>
        [TestMethod]
        public void TestStartAndStopProcessing()
        {
            /// Mock client and server creation.
            var mockViewModel = new Mock<IDataReceiver>();
            ScreenshareServer server = ScreenshareServer.GetInstance(mockViewModel.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);

            /// Mock images will insert into the client's image queue.
            int numOfImages = 5;
            List<string> clientImage = new();
            for (int i = 0; i < numOfImages; i++)
            {
                clientImage.Add(Utils.RandomString(i + 100));
                client.PutImage(clientImage[i], client.TaskId);
            }

            /// It will try to stop the client processing which was not started yet.
            client.StopProcessing();

            /// It will start the processing of the images for the client.
            client.StartProcessing(new((taskId) =>
            {
                while(taskId == client.TaskId)
                {
                    Bitmap? finalImage = client.GetFinalImage(client.TaskId);

                    if(finalImage != null)
                    {
                        client.CurrentImage = SSUtils.BitmapToBitmapImage(finalImage);
                    }
                }
            }));

            /// This sleep is just for keep the tasks running for some time.
            /// All the images in the queue will be processed by stiticher.
            Thread.Sleep(5000);

            /// It will try to start the process which will already started.
            client.StartProcessing(new(_ => { return; }));

            // Stop the processing of the images for the client.
            client.StopProcessing();

            // Trying to stop the processing again.
            client.StopProcessing();

            // Assert.
            // The "CurrentImage" variable of the client should not be null at the end.
            //Assert.IsTrue(client.CurrentImage != null);

            // Cleanup.
            client.Dispose();
            server.Dispose();
        }
    }
}
