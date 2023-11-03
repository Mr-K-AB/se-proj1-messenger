using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MessengerNetworking.Serializer;

namespace MessengerTests.MessengerNetworking.Serializer
{
    [TestClass]
    public class SerializerTest
    {
        [TestMethod]
        public void TestSerializeAndDeserialze()
        {
            // Arrange
            var objToSerialize = new SampleObject { Id = 1 , name = "TestObject" };

            ISerializer serializer = new Serialzer();
            string serializedData = serializer.Serialize<SampleObject>( objToSerialize );

            Assert.IsNotNull( serializedData );

            SampleObject deserializedObj = serializer.Deserialize<SampleObject>( serializedData );

            Assert.IsNotNull( deserializedObj );

            Assert.AreEqual<SampleObject>( deserializedObj, objToSerialize );
        }
    }
}

public class SampleObject
{
    public int Id { get; set; }
    public string name { get; set; }
}
