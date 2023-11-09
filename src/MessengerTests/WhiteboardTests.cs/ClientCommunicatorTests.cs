using System.Diagnostics;
using System.Xml.Serialization;
using MessengerDashboard.Summarization;
using MessengerDashboard;
using MessengerDashboard.Sentiment;

namespace MessengerTests
{
    [TestClass]
    public class ClientCommunicatorTests
    {
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
        //[TestMethod]
    }
}
