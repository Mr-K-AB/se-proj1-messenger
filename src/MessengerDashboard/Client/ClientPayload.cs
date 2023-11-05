using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Dashboard
{
    public class ClientPayload
    {

        public Operation eventType;
        public int userID;
        public string username;
        public string userEmail;
        public string photoUrl;

        //parametrized constructor 
        public ClientPayload(Operation eventName, string clientName, int clientID = -1, string clientEmail = null, string clientPhotoUrl = null)
        {
            eventType = eventName;
            username = clientName;
            userID = clientID;
            userEmail = clientEmail;
            photoUrl = clientPhotoUrl;
        }

        //default constructor for serialization
        public ClientPayload()
        {

        }
    }
}
