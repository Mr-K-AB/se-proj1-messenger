/******************************************************************************
* Filename    = SummarizerTests.cs
*
* Author      = Pratham Ravindra Nagpure 
*
* Roll number = 112001054
*
* Product     = Messenger 
* 
* Project     = MessengerTests
*
* Description = A class to test the summarizer.
*****************************************************************************/

using System.Diagnostics;
using System.Xml.Serialization;
using MessengerDashboard.Summarization;
using MessengerDashboard;
using MessengerDashboard.Sentiment;

namespace MessengerTests.DashboardTests
{
    [TestClass]
    public class SummarizerTests
    {
        [TestMethod]
        public void TextSummarizerTest()
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
                    MaxSummaryPercentage = 100,
                    MaxSummarySentences = 3
                };
                TextSummary summary = summarizer.Summarize(new string[] { "America is a country", "I am OK", "India is a country", }, request);
                Assert.IsNotNull(summary);
                Assert.AreEqual(summary.Sentences[2], "I am OK");
            }
            Assert.IsNotNull(new TextSummary());
        }

        [TestMethod]
        public void TextSummarizerTest2()
        {
            ITextSummarizer summarizer = TextSummarizerFactory.GetTextSummarizer();
            if (summarizer is null)
            {
                Assert.Fail();
            }
            else
            {
                TextSummarizationOptions request = new();
                TextSummary summary = summarizer.Summarize(new string[] { 
                    "Food is good",
                    "Food is the most important part of life",
                    "There are nine balls", 
                }, request);
                Assert.IsNotNull(summary);
                Assert.AreEqual(summary.Sentences[0], "Food is good");
                Assert.AreEqual(summary.Sentences[1], "Food is the most important part of life");
            }
        }

    }
}
