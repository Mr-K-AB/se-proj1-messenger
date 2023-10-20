using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Dashboard.Client.Session
{
    public class UserSessionManagement
    {
        public delegate void NotifyEndMeet();

      //  public delegate void NotifyAnalyticsCreated( SessionAnalytics analytics );

        public delegate void NotifySummaryCreated( string summary );

        public delegate void NotifySessionTypeChanged( string sessionMode );
    }
}
