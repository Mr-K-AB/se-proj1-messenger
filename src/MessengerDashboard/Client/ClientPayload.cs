/******************************************************************************
 * Filename    = ClientPayload.cs
 *
 * Author      = Shailab Chauhan 
 *
 * Roll number = 112001038
 *
 * Product     = Messenger 
 * 
 * Project     = MessengerDashboard
 *
 * Description = A class that stores the Payload sent  from the client.
 *****************************************************************************/

namespace MessengerDashboard.Client
{
    /// <summary>
    /// Represents a payload sent from the client to the server.
    /// </summary>
    public class ClientPayload
    {
        /// <summary>
        /// Gets or sets the operation type in the payload.
        /// </summary>
        public Operation Operation { get; set; }

        /// <summary>
        /// Gets or sets user information associated with the payload.
        /// </summary>
        public UserInfo UserInfo { get; set; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientPayload"/> class.
        /// </summary>
        public ClientPayload()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientPayload"/> class with specified operation and user information.
        /// </summary>
        /// <param name="eventName">The operation type.</param>
        /// <param name="userInfo">User information.</param>
        public ClientPayload(Operation eventName, UserInfo? userInfo)
        {
            Operation = eventName;
            UserInfo = userInfo;
        }
    }
}
