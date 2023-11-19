using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerScreenshare.Server;

namespace MessengerTests.ScreenshareTests
{
    [TestClass]
    public class SharedClientScreenTest
    {
        /// <summary>
        /// Update time values after the timeout time.
        /// </summary>
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


    }
}
