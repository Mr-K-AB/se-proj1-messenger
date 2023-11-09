using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MessengerDashboard
{
    public class Serializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public Serializer()
        {
            _jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        }

        public string Serialize<T>(T objectToSerialize)
        {
            string json = SerializeJson(objectToSerialize);
            SerializedDataWrapper obj = new() { DataType = typeof(T), SerializedData = json };
            return SerializeJson(obj);
        }

        public T Deserialize<T>(string serializedString)
        {
            SerializedDataWrapper obj = DeserializeJson<SerializedDataWrapper>(serializedString);
            return DeserializeJson<T>(obj.SerializedData);
        }

        /// <summary>
        ///     JSON supported serialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        private string SerializeJson<T>(T objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented, _jsonSerializerSettings);
        }

        /// <summary>
        ///     JSON supoorted deserialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        private T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
        }
    }
}
