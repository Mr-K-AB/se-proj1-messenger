using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MessengerCloud;

namespace MessengerTests
{
    [TestClass]
    public class BasicTest
    {
        
        private AnalysisCloud _analysisCloud;
        [TestMethod]
        public void Test1()
        {
            _analysisCloud = new(
                 new()
                ,
                 new()
                ,
                 2
                ,
                 2
                );
            //_analysisCloud.TimeStampToUserCountMap.Add(DateTime.Now, 3);
            _analysisCloud.UserIdToUserActivityMap.Add(4, new UserActivityCloud() { /*EntryTime = DateTime.Now, ExitTime = DateTime.Now, */UserEmail = "hello", UserChatCount = 2, UserName = "shubh" });
            string ans = JsonSerializer.Serialize( _analysisCloud );
            AnalysisCloud res = JsonSerializer.Deserialize<AnalysisCloud>( ans );
            Assert.IsTrue( res != null );


        }


    }
}
