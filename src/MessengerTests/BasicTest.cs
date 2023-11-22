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
using System.Text.Json;
using System.Threading.Tasks;
using MessengerCloud;
using MessengerDashboard;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace MessengerTests
{
    /// <summary>
    /// Basic tests for small methods .
    /// </summary>
    [TestClass]
    public class BasicTest
    {
        
        
        private Analysis _analysisCloud;
        readonly List<string> _sentences = new() { "Hi", "Hello", "Wow" };
        //private readonly LocalSave _save = new();
        public BasicTest()
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
            _analysisCloud.TimeStampToUserCountMap.Add(DateTime.Now, 3);
            _analysisCloud.UserIdToUserActivityMap.Add(4, new UserActivity() { EntryTime = DateTime.Now, ExitTime = DateTime.Now, UserEmail = "hello", UserChatCount = 2, UserName = "shubh" });
        }
        /// <summary>
        /// checking if the class is serializable or not  
        /// </summary>
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
            _analysisCloud.UserIdToUserActivityMap.Add(4, new UserActivity() { /*EntryTime = DateTime.Now, ExitTime = DateTime.Now, */UserEmail = "hello", UserChatCount = 2, UserName = "shubh" });
            string ans = JsonSerializer.Serialize( _analysisCloud );
            Analysis res = JsonSerializer.Deserialize<Analysis>(ans);
            Assert.IsTrue( res != null );


        }



        /// <summary>
        /// Tests creating and getting an local entity.
        /// </summary>
        [TestMethod]
        public async Task LocalSaveTesting()
        {
            // delete all 
            Logger.LogMessage("Deleting all entries from our Azure table storage.");

            // Create an entity.
            Logger.LogMessage("Create an entity.");
            var info = new EntityInfoWrapper(_sentences, 1, 2, 3, "Positive", "-1", _analysisCloud);
            List<EntityInfoWrapper> infos = new()
            {
                info
            };

            LocalSave.AddEntity(info);
            List<EntityInfoWrapper> res = LocalSave.ReadFromFile();
            Assert.AreEqual(JsonSerializer.Serialize(infos[0].Sentences), JsonSerializer.Serialize(res[0].Sentences));
        }






    }
}
