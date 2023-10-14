using System.Diagnostics;
using System.Xml.Serialization;


using MessengerDashboard;

namespace MessengerTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Task<List<string>> task =Authenticator.Authenticate();
            task.Wait();
            var result = task.Result;
            if (result[0] == "false" ) { 

                Debug.WriteLine("not working ");
                Assert.Fail("this is the reason "); }
            else
            {
                Debug.WriteLine(result);
            }
        }
    }
}