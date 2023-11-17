using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApp
{
    public class User
    {
        public string UserName
        {
            get; set;

        }
        public string UserPicturePath { get; set; }

        public User (string userName, string userPicturePath)
        {
            UserName = userName;
            UserPicturePath = userPicturePath;
        }
    }
}
