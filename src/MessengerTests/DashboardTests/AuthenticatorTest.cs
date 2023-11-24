/******************************************************************************
* Filename    = AuthenticatorTest.cs
*
* Author      = Shubh Pareek 
*
* Roll number = 112001039
*
* Product     = Messenger 
* 
* Project     = MessengerTests
*
* Description = A class to test the Google authenticator.
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard;

namespace MessengerTests.DashboardTests
{
    [TestClass]
    public class AuthenticatorTest
    {
        /// <summary>
        /// Checks if authentication is working or not .
        /// </summary>
        [TestMethod]
        public void AuthenticationTest()
        {
            Task<AuthenticationResult> task = Authenticator.Authenticate();
            task.Wait();
            AuthenticationResult result = task.Result;
            Assert.IsNotNull(result);
            if (!result.IsAuthenticated)
            {
                Assert.Fail("Authentication Failed");
            }
            Assert.IsNotNull(result.UserName);
            Assert.IsNotNull(result.UserImage);
            Assert.IsNotNull(result.UserEmail);
        }

        [TestMethod]
        public void AuthenticationFailTest()
        {
            Task<AuthenticationResult> task = Authenticator.Authenticate(0);
            task.Wait();
            AuthenticationResult result = task.Result;
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsAuthenticated);
        }
    }
}
