using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Telemetry;
using MessengerDashboard.Summarization;
using MessengerDashboard.Client;

namespace MessengerDashboard.Server
{
    public class ServerPayload
    {
        public UserInfo _user;
        public Operation eventType;
        public AnalysisResults sessionAnalytics;
        public SessionInfo sessionData;
        public TextSummary summaryDetail;

        //     Parametric constructor to initialize the fields
        public ServerPayload(Operation eventName, SessionInfo sessionDataToSend, TextSummary summaryDataToSend,
            AnalysisResults sessionAnalyticsToSend, UserInfo user)
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
