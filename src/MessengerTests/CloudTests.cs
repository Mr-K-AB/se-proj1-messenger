using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MessengerCloud;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;


namespace MessengerTests
{
    [TestClass]
    public class CloudTests
    {
        private const string BaseUrl = @"http://localhost:7166/api/entity";
        private readonly RestClient _restClient;
        private readonly AnalysisCloud _analysisCloud;
        readonly List<string> _sentences = new() { "Hi", "Hello", "Wow" };


        public CloudTests()
        {
            _restClient = new(BaseUrl);
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
            _analysisCloud.UserIdToUserActivityMap.Add(4, new UserActivityCloud() { EntryTime = DateTime.Now, ExitTime = DateTime.Now, UserEmail = "hello", UserChatCount = 2, UserName = "shubh" });
        }

        /// <summary>
        /// Tests creating and getting an entity.
        /// </summary>
        [TestMethod]
        public async Task TestPostAndGet()
        {
            // delete all 
            Logger.LogMessage("Deleting all entries from our Azure table storage.");
            await _restClient.DeleteEntitiesAsync();

            // Create an entity.
            Logger.LogMessage("Create an entity.");
            var info = new EntityInfoWrapper(_sentences, 1, 2, true, "-1", _analysisCloud);
            Entity? postEntity = await _restClient.PostEntityAsync(info);
            // Get the entity.
            Logger.LogMessage("Get the entity.");
            Entity? getEntity = await _restClient.GetEntityAsync(postEntity?.Id);

            // Validate.
            Logger.LogMessage("Validate.");
            Assert.AreEqual(postEntity?.Id, getEntity?.Id);
            //Assert.AreEqual(postEntity?.Name, getEntity?.Name);
            //now delete
            Logger.LogMessage("Delete the entity.");
            await _restClient.DeleteEntityAsync(postEntity?.Id);

            // Validate.
            // Trying to get the entity should throw an exception.
            try
            {
                // Get the entity.
                Logger.LogMessage("Getting the entity.");
                getEntity = await _restClient.GetEntityAsync(postEntity?.Id);
                Assert.Fail("Trying to get a deleted entity did not throw an exception.");
            }
            catch (HttpRequestException httpEx) when (httpEx.StatusCode == HttpStatusCode.NotFound)
            {
                Logger.LogMessage("Rightly got the expected exception trying to get a deleted entity.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Unexpected exception type. Message = {ex.Message}");
            }
        }
        [TestMethod]
        public async Task TestDeleteAllAndGetAll()
        {
            // Delete any existing entities.
            Logger.LogMessage("Delete any existing entities.");
            await _restClient.DeleteEntitiesAsync();

            // Create three entities.
            Logger.LogMessage("Create three entities.");
            _ = await _restClient.PostEntityAsync(new EntityInfoWrapper(_sentences, 1, 2, true, "-1", _analysisCloud));
            _ = await _restClient.PostEntityAsync(new EntityInfoWrapper(_sentences, 1, 2, true, "0", _analysisCloud));
            _ = await _restClient.PostEntityAsync(new EntityInfoWrapper(_sentences, 1, 2, true, "1", _analysisCloud));

            // Validate.
            Logger.LogMessage("Validate.");
            IReadOnlyList<Entity>? entities = await _restClient.GetEntitiesAsync();
            Assert.AreEqual(entities?.Count, 3);
            // Delete any existing entities.
            Logger.LogMessage("Delete any existing entities.");
            await _restClient.DeleteEntitiesAsync();
        }



    }



}
