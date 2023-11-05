using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Dashboard
{
    public class ServerPayload
    {
        public UserInfo _user;
        public string eventType;
        public SessionAnalytics sessionAnalytics;
        public SessionInfo sessionData;
        public SummaryDetail summaryDetail;

        //     Parametric constructor to initialize the fields
        public ServerPayload(string eventName, SessionInfo sessionDataToSend, SummaryDetail summaryDataToSend,
            SessionAnalytics sessionAnalyticsToSend, UserInfo user)
        {
            // SessionAnalytics sessionAnalyticsToSend
            eventType = eventName;
            _user = user;
            sessionData = sessionDataToSend;
            summaryDetail = summaryDataToSend;
            sessionAnalytics = sessionAnalyticsToSend;
        }

        //     Default constructor for serialization
        public ServerPayload()
        {
        }

        //     Method to access the UserData object
        public UserInfo GetUser()
        {
            return _user;
        }
    }
}
