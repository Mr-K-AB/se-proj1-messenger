using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
