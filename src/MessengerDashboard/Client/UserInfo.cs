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
        public string? UserName { get; set; }
        public int UserID { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhotoUrl { get; set; }

        public UserInfo()
        {

        }

        public UserInfo(string clientName, int clientID, string? clientEmail = null, string? clientGender = null, string? clientStatus = null, string? clientPhotoUrl = null)
        {
            UserID = clientID;
            UserName = clientName;
            UserEmail = clientEmail;
            UserPhotoUrl = clientPhotoUrl;
        }

        public bool Equals(UserInfo client)
        {
            if (client == null)
            {
                return false;
            }

            return UserID.Equals(client.UserID) &&
                   (ReferenceEquals(UserName, client.UserName) ||
                    UserName != null && UserName.Equals(client.UserName));
        }
    }
}

