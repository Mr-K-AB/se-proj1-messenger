/******************************************************************************
 * 
 * Author      = Vikas Saini
 *
 * Roll no     = 112001049
 *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerNetworking.Serializer
{
    /// <summary>
    /// Class has functions for serialization and deserialization
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Function takes and object and serializes it 
        /// and converts to string
        /// </summary>
        /// <typeparam name="type"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Serialize<type>(type data);

        /// <summary>
        /// Function takes serializedData and deserializes it (return original object)
        /// </summary>
        /// <typeparam name="type"></typeparam>
        /// <param name="serializedData"></param>
        /// <returns></returns>
        public type Deserialize<type>(string serializedData);
    }
}
