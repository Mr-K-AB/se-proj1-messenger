/******************************************************************************
* Filename    = User.cs
*
* Author      = Satish Patidar 
*
* Roll number = 112001037
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = Represents a user in the MessengerDashboard application.
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.UI.DataModels
{
    /// <summary>
    /// Represents a user in the MessengerDashboard application.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the user's username.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the path to the user's profile picture.
        /// </summary>
        public string UserPicturePath { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class with the specified username and profile picture path.
        /// </summary>
        /// <param name="userName">The username of the user.</param>
        /// <param name="userPicturePath">The path to the user's profile picture.</param>
        public User(string userName, string userPicturePath)
        {
            UserName = userName;
            UserPicturePath = userPicturePath;
        }
    }
}
