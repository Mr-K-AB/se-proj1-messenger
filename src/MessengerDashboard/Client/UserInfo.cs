/******************************************************************************
* Filename    = UserInfo.cs
*
* Author      = Shailab Chauhan 
*
* Roll number = 112001038
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = Represents information about a user.
*****************************************************************************/

namespace MessengerDashboard.Client
{
    /// <summary>
    /// Represents information about a user.
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfo"/> class.
        /// </summary>
        public UserInfo() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfo"/> class with specified parameters.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="userEmail">The email of the user (optional).</param>
        /// <param name="userPhotoUrl">The URL of the user's photo (optional).</param>
        public UserInfo(
            string userName,
            int userId,
            string? userEmail = null,
            string? userPhotoUrl = null
        )
        {
            UserId = userId;
            UserName = userName;
            UserEmail = userEmail;
            UserPhotoUrl = userPhotoUrl;
        }

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        public string UserEmail { get; set; } = "";

        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public int UserId { get; set; } = -1;

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string UserName { get; set; } = "";

        /// <summary>
        /// Gets or sets the URL of the user's photo.
        /// </summary>
        public string UserPhotoUrl { get; set; } = "";

    }
}

