﻿/******************************************************************************
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
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics;

namespace MessengerNetworking.Serializer
{
    public class Serialzer : ISerializer
    {
        public string Serialize<type>( type data )
        {
            try
            {
                var serializer = new XmlSerializer( typeof( type ) );
                using var stringWriter = new StringWriter();
                using (var xmlWriter = XmlWriter.Create( stringWriter , new XmlWriterSettings { Indent = true } ))
                {
                    serializer.Serialize( xmlWriter , data );
                }
                Trace.TraceInformation("Networking Serialised");
                return stringWriter.ToString();
            }
            catch (Exception ex)
            {
                throw new SerializationException( "Error occurred during serialization." , ex );
            }
        }

        public type Deserialize<type>( string serializedData )
        {
            try
            {
                var serializer = new XmlSerializer( typeof( type ) );
                using var stringReader = new StringReader( serializedData );
                using var xmlReader = XmlReader.Create( stringReader );
                Trace.TraceInformation("Networking Deserialised");
                return (type)serializer.Deserialize( xmlReader );
            }
            catch (Exception ex)
            {
                throw new SerializationException( "Error occurred during deserialization." , ex );
            }
        }
    }
    public class SerializationException : Exception
    {
        public SerializationException(string message, Exception innerException) : base(message, innerException) { }
    }

}


