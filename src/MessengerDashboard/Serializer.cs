/******************************************************************************
* Filename    = Serializer.cs
*
* Author      = Shailab Chauhan 
*
* Roll number = 112001038
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = Serializing and Deserializing
*****************************************************************************/
using Newtonsoft.Json;

namespace MessengerDashboard
{
    /// <summary>
    /// Provides methods for serializing and deserializing objects using JSON.
    /// </summary>
    public class Serializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Serializer"/> class.
        /// </summary>
        public Serializer()
        {
            _jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        }

        /// <summary>
        /// Serializes an object to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="objectToSerialize">The object to serialize.</param>
        /// <returns>The JSON representation of the serialized object.</returns>
        public string Serialize<T>(T objectToSerialize)
        {
            string json = SerializeJson(objectToSerialize);
            SerializedDataWrapper obj = new() { DataType = typeof(T), SerializedData = json };
            return SerializeJson(obj);
        }

        /// <summary>
        /// Deserializes a JSON string to an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="serializedString">The JSON string to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        public T Deserialize<T>(string serializedString)
        {
            SerializedDataWrapper obj = DeserializeJson<SerializedDataWrapper>(serializedString);
            return DeserializeJson<T>(obj.SerializedData);
        }

        /// <summary>
        /// JSON-supported serialization.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="objectToSerialize">The object to serialize.</param>
        /// <returns>The JSON representation of the serialized object.</returns>

        private string SerializeJson<T>(T objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented, _jsonSerializerSettings);
        }

        /// <summary>
        /// JSON-supported deserialization.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        private T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
        }
    }
}
