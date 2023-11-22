/// <credits>
/// <author>
/// <name>Shailab Chauhan</name>
/// <rollnumber>112001038</rollnumber>
/// </author>
/// </credits>

using System;

namespace MessengerDashboard.Client
{
    public class UserInfo
    {
        public UserInfo()
        {

        }

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

        public string? UserEmail { get; set; }

        public int UserId { get; set; }

        public string? UserName { get; set; }

        public string? UserPhotoUrl { get; set; }

        public bool Equals(UserInfo client)
        {
            if (client == null)
            {
                return false;
            }

            return UserId.Equals(client.UserId) &&
                   (ReferenceEquals(UserName, client.UserName) ||
                    UserName != null && UserName.Equals(client.UserName));
        }
    }
}

