﻿/// <author>Likhith Reddy</author>
/// <summary>
/// Defines the enum "ServerDataHeader", which enumerates all the headers
/// that could be present in the data packet sent by the server.
/// </summary>

using System.Runtime.Serialization;

namespace MessengerScreenshare
{
    /// <summary>
    /// Enumerates all the headers that could be present in the data packet
    /// sent by the server.
    /// </summary>
    public enum ServerDataHeader
    {
        /// <summary>
        /// Ask/Tell the client to start sending Image data packets
        /// with the given resolution.
        /// </summary>
        [EnumMember(Value = "SEND")]
        Send,

        /// <summary>
        /// Ask the client to stop sending Image data packets.
        /// </summary>
        [EnumMember(Value = "STOP")]
        Stop,

        /// <summary>
        /// Confirmation packet sent to the client.
        /// </summary>
        [EnumMember(Value = "CONFIRMATION")]
        Confirmation
    }
}