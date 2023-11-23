/******************************************************************************
 * Filename    = MessageEvent.cs
 *
 * Author      = Manikanta
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContent
 *
 * Description = Types of message events
 *****************************************************************************/

namespace MessengerContent.Enums
{
    /// <summary>
    /// Type of message event - New, Edit, Delete, Star, Download.
    /// </summary>
    public enum MessageEvent
    {
        New,
        Edit,
        Delete,
        Star,
        Download
    }
}
