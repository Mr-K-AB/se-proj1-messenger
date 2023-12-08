/******************************************************************************
* Filename    = AuthenticationResult.cs
*
* Author      = Shubh Pareek
*
* Roll Number = 112001039
*
* Product     = Messenger 
* 
* Project     = Dashboard
*
* Description = for authentication of user .
* *****************************************************************************/

namespace MessengerDashboard
{
    public class AuthenticationResult
    {
        public bool IsAuthenticated { get; set; } = false;

        public string? UserName { get; set; } = null;

        public string? UserEmail { get; set; } = null;

        public string? UserImage { get; set; } = null;
    }
}
