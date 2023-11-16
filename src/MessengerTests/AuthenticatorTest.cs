/// <credits>
/// <author>
/// <name>Shubh Pareek</name>
/// <rollnumber>112001039</rollnumber>
/// </author>
/// </credits>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard;

namespace MessengerTests
{
    [TestClass]
    public class AuthenticatorTest
    {
        /// <summary>
        /// checks if authentication is working or not .
        /// </summary>
        [TestMethod]
        public void AuthenticationTest()
        {
            Task<AuthenticationResult> task = Authenticator.Authenticate();
            task.Wait();
            AuthenticationResult result = task.Result;
            if (!result.IsAuthenticated)
            {
                Assert.Fail("Authentication Failed");
            }

        }

    }
}
