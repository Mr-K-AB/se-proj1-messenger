/******************************************************************************
 * Filename    = ScreenStitcher.cs
 *
 * Author      = Aditya Raj
 *
 * Product     = Messenger
 * 
 * Project     = MessengerScreenshare
 *
 * Description = Contains Tests for ScreenStitcher
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerScreenshare.Client;
using MessengerScreenshare.Server;
using Moq;

namespace MessengerTests.ScreenshareTests
{
    [TestClass]
    public class ScreenStitcherTest
    {
        private byte[] CompressByteArray(byte[] data)
        {
            MemoryStream output = new ();
            using (DeflateStream dstream = new (output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        /// <summary>
        /// Test to check the Decompression Algorithm
        /// </summary>
        [TestMethod]
        public void DecompressByteArrayTest()
        {
            byte[] arr = { (byte)'a', (byte)'d', (byte)'i', (byte)'t', (byte)'y', (byte)'a' };
            byte[] compressedByte = CompressByteArray(arr);

            byte[] decompressedByte = ScreenStitcher.DecompressByteArray(compressedByte);

            string expected = BitConverter.ToString(arr);
            string actual = BitConverter.ToString(decompressedByte);
            Assert.IsTrue(string.Equals(expected, actual));
        }

        /// <summary>
        /// Test to check the XOR operation in Process method
        /// </summary>
        [TestMethod]
        public void ProcessTest()
        {
            Bitmap? img = Screenshot.Instance().MakeScreenshot();
            if (img == null)
            {
                Assert.Fail("Failed to capture the screenshot.");
                return;
            }
            Bitmap curImg = new(img);

            Bitmap diff = ScreenStitcher.Process(img, curImg);
            Bitmap undoDiff = ScreenStitcher.Process(img, diff);

            Assert.IsNotNull(undoDiff);
            Assert.IsNotNull(diff);

            Assert.IsTrue(Utils.CompareBitmap(img, curImg));

        }

    }
}
