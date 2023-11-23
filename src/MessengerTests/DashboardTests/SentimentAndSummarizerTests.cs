/******************************************************************************
* Filename    = SentimentAndSummarizerTests.cs
*
* Author      = Pratham Ravindra Nagpure 
*
* Roll number = 112001054
*
* Product     = Messenger 
* 
* Project     = MessengerTests
*
* Description = A class to test the sentiment and the summarizer.
*****************************************************************************/

using System.Diagnostics;
using System.Xml.Serialization;
using MessengerDashboard.Summarization;
using MessengerDashboard;
using MessengerDashboard.Sentiment;

namespace MessengerTests.DashboardTests
{
    [TestClass]
    public class SentimentAndSummarizerTests
    {
        [TestMethod]
        public void TextSummarizerFactoryTest()
        {
            ITextSummarizer summarizer = TextSummarizerFactory.GetTextSummarizer();
            if (summarizer is null)
            {
                Assert.Fail();
            }
            else
            {
                TextSummarizationOptions request = new()
                {
                    MaxSummaryPercentage = 100
                };
                TextSummary summary = summarizer.Summarize(new string[] { "America is a country", "I am OK", "India is a country", }, request);
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
            if (sentimentAnalyzer is null)
            {
                Assert.Fail();
            }
            else
            {
                SentimentResult result = sentimentAnalyzer.AnalyzeSentiment(new string[] { "Very good man", "I don't like that", "Never go there" });
                if (result.PositiveChatCount != 1 || result.NegativeChatCount != 2 || result.OverallSentiment == "Positive")
                {
                    Assert.Fail();
                }
            }
        }

    }
}
