/*************************************************************************************
* Filename    = ClientDataHeader.cs
*
* Author      = Likhith Reddy
*
* Product     = ScreenShare
* 
* Project     = Messenger
*
* Description = Defines the enumerator "ClientDataHeader", which enumerates all the
*               headers that could be present in the data packet sent by the client.
*************************************************************************************/
using System.Runtime.Serialization;

namespace MessengerScreenshare
{
    /// <summary>
    /// Enumerates all the headers that could be present
    /// in the data packet sent by the client.
    /// </summary>
    public enum ClientDataHeader
    {
        
        // Register a client for screen sharing.
        [EnumMember(Value = "REGISTER")]
        Register,

        // De-register a client for screen sharing.
        [EnumMember(Value = "DEREGISTER")]
        Deregister,

        // Image received from the client.
        [EnumMember(Value = "IMAGE")]
        Image,

        // Confirmation packet received from the client.
        [EnumMember(Value = "CONFIRMATION")]
        Confirmation
    }
}
