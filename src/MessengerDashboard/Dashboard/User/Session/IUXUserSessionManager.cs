using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Dashboard.User.Session
{
    public interface IUXUserSessionManager
    {

        bool AddUser();
        void ToggleSessionType();

        void RemoveUser();

        void EndMeeting();

        void GetSummary();


        void SubscribeSession( IUserNotification listener );

        void GetAnalytics();

        UserInfo GetUser();

        SessionInfo GetSessionInfo();


    }
}
