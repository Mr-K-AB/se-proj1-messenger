/******************************************************************************
* Filename    = ContentSerializer.cs
*
* Author      = Likhitha
*
* Product     = Messenger
* 
* Project     = MessengerContent
*
* Description = serealizer for messages
*****************************************************************************/
using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using TraceLogger;

namespace MessengerContent
{
    /// <summary>
    ///     Wrapper object to store serialized object's type and serialized string representation.
    /// </summary>
    public class MetaObject
    {
        public string data;
        public string typ;

        public MetaObject()
        {
        }

        public MetaObject(string typ, string data)
        {
            this.data = data;
            this.typ = typ;
        }
    }
    public class ContentSerializer : IContentSerializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public ContentSerializer()
        {
            _jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        }

        /// <inheritdoc />
        string IContentSerializer.Serialize<T>(T objToSerialize)
        {
            try
            {
                string json = SerializeJson(objToSerialize);
                var obj = new MetaObject(typeof(T).ToString(), json);
                return SerializeJson(obj);
            }
            catch (Exception ex)
            {
                Logger.Log($" Error while serializing: {ex.Message}", LogLevel.WARNING);
                throw;
            }
        }

        /// <inheritdoc />
        public string GetObjType(string serializedString, string nameSpace)
        {
            // json string
            MetaObject obj = DeserializeJson<MetaObject>(serializedString);
            return obj.typ;
        }

        /// <inheritdoc />
        public T Deserialize<T>(string serializedString)
        {
            try
            {
                MetaObject obj = DeserializeJson<MetaObject>(serializedString);
                return DeserializeJson<T>(obj.data);
            }
            catch (Exception ex)
            {
                Logger.Log($" Error while deserializing: {ex.Message}", LogLevel.WARNING);
                throw;
            }
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
        ///     JSON supported deserialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        private T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}


