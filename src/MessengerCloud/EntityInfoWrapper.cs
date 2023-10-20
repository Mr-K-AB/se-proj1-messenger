using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessengerCloud
{
    public class EntityInfoWrapper
    {
        public EntityInfoWrapper(List<string> sentences,int positiveChatCount,int negativeChatCount, bool isOverallSentimentPositive ) { 
        
            Sentences = sentences;
            PositiveChatCount = positiveChatCount;
            NegativeChatCount = negativeChatCount;
            IsOverallSentimentPositive = isOverallSentimentPositive;
        
        }

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

    }
}
