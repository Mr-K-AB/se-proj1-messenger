/******************************************************************************
 * Filename    = MessageType.cs
 *
 * Author      = Likhitha
 *
 * Product     = Messanger
 * 
 * Project     = MessangerContent
 *
 * Description = Message type
 *****************************************************************************/

namespace MessengerContent
{
    /// <summary>
    /// Type of message - Chat or File.
    /// </summary>
    public enum MessageType
    {
        File,
        Chat,
        HistoryRequest
    }
}
