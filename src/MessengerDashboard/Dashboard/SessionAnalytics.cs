using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Telemetry;

namespace MessengerDashboard.Dashboard
{
    public class SessionAnalytics
    {

        //stores chat count per user 
        public Dictionary<int, int> chatCountForEachUser;


        //stores chat count vs user name 
        public Dictionary<string, int> userNameVsChatCount;

        //stores the id of insincere members 
        public List<int> listOfInSincereMembers;


        //stores user count at each time stamp whenever list of users is updated 
        public Dictionary<DateTime, int> userCountVsTimeStamp;

        //stores the some ad hoc data about the session like the sessions score, chat count 
        public SessionDetail sessionDetail;
    }
}
