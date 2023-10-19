using System.Diagnostics;
using System.Xml.Serialization;
using MessengerDashboard.Summarization;


using MessengerDashboard;
using MessengerDashboard.Sentiment;

namespace MessengerTests
{
    [TestClass]
    public class DashboardTests
    {
        [TestMethod]
        public void AuthenticationTest()
        {
            Task<List<string>> task =Authenticator.Authenticate();
            task.Wait();
            List<string> result = task.Result;
            if (result[0] == "false" ) { 

                Debug.WriteLine("not working ");
                Assert.Fail("this is the reason "); }
            else
            {
                Debug.WriteLine(result);
            }
        }

        [TestMethod]
        public void TextSummarizerFactoryTest()
        {
            ITextSummarizer summarizer = TextSummarizerFactory.GetTextSummarizer();
            if(summarizer is null)
            {
                Assert.Fail();
            }
            else
            {
                TextSummarizationOptions request = new()
                {
                    MaxSummaryPercentage = 100
                };
                TextSummary summary = summarizer.Summarize(new string[] {"America is a country", "I am OK", "India is a country",}, request);
                if (summary is null || summary.Sentences[2] != "I am OK")
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void SentimentAnalyzerTest()
        {
            ISentimentAnalyzer sentimentAnalyzer = SentimentAnalyzerFactory.GetSentimentAnalyzer();
            if(sentimentAnalyzer is null)
            {
                Assert.Fail();
            }
            else
            {
                SentimentResult result = sentimentAnalyzer.AnalyzeSentiment(new string[] { "Very good man", "I don't like that", "Never go there" });
                if (result.PositiveChatCount != 1 || result.NegativeChatCount != 2 || result.IsOverallSentimentPositive == true)
                {
                    Assert.Fail();
                }
            }
        }

    }
}
