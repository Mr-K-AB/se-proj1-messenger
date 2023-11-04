/******************************************************************************
* Filename    = ContentServerNotificationHandler.cs
* Author      = 
* Product     = Messenger
* Project     = MessengerContent
* Description = 
*****************************************************************************/

using MessengerNetwork;
namespace MessengerContent.Server
{
    public class ContentServerNotificationHandler : INotificationHandler
    {
        public readonly ContentServer ContentServer;
        public ContentServerNotificationHandler(ContentServer contentServer)
        {
            ContentServer = contentServer;
        }
        /// <inheritdoc />
        public void OnDataReceived(string data)
        {
            ContentServer.Receive(data);
        }
    }
}
