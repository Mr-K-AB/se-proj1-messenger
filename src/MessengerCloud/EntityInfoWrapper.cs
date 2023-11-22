/// <credits>
/// <author>
/// <name>Shubh Pareek</name>
/// <rollnumber>112001039</rollnumber>
/// </author>
/// </credits>
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessengerCloud
{
    public class EntityInfoWrapper
    {
        public EntityInfoWrapper(List<string> sentences,int positiveChatCount,int negativeChatCount, bool isOverallSentimentPositive ,string sessionId, AnalysisCloud obj) { 
        
            Sentences = sentences;
            PositiveChatCount = positiveChatCount;
            NegativeChatCount = negativeChatCount;
            IsOverallSentimentPositive = isOverallSentimentPositive;
            SessionId = sessionId;

            Analysis = obj;
            Trace.WriteLine("[EntityInfoWrapper]: wrapper created");
        }
        public EntityInfoWrapper(){}
        [JsonInclude]
        [JsonPropertyName("SessionId")] //Unique id for the session conducted
        public string SessionId { get; set; }


        [JsonInclude]
        [JsonPropertyName("Sentences")]
        public List<string> Sentences;
        [JsonInclude]
        [JsonPropertyName("PositiveChatCount")]
        public int PositiveChatCount { get; set; }
        [JsonInclude]
        [JsonPropertyName("NegativeChatCount")]
        public int NegativeChatCount { get; set; }
        [JsonInclude]
        [JsonPropertyName("IsOverallSentinmentPositive")]
        public bool IsOverallSentimentPositive { get; set; }

        [JsonInclude]
        [JsonPropertyName("Analysis")]
        public AnalysisCloud Analysis { get; set; }





    }
}
