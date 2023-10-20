/// <credits>
/// <author>
/// <name>Shailab Chauhan</name>
/// <rollnumber>112001038</rollnumber>
/// </author>
/// </credits>

using System;

namespace MessengerDashboard.Dashboard
{
    public class UserInfo
    {
        public string userName;
        public int userID;
        public string? userEmail;
        public string? userGender;
        public string? userStatus;
        public string? userPhotoUrl;

        public UserInfo()
        {

        }

        public UserInfo( string clientName , int clientID , string? clientEmail = null , string? clientGender = null , string? clientStatus = null , string? clientPhotoUrl = null )
        {
            userID = clientID;
            userName = clientName;
            userEmail = clientEmail;
            userGender = clientGender;
            userStatus = clientStatus;
            userPhotoUrl = clientPhotoUrl;
        }

        public bool Equals( Userinfo client )
        {
            if (client == null)
            {
                return false;
            }

            return userID.Equals( client.userID ) &&
                   (
                       ReferenceEquals( userName , client.userName ) ||
                       userName != null &&
                       userName.Equals( client.userName )
                   );
        }
    }


}
