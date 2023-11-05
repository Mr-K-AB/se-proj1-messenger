using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Dashboard;

namespace MessengerDashboard.Client
{
    public interface IClientSessionController
    {

        bool AddUser();
        void ToggleSessionType();

        void RemoveUser();

        void EndMeeting();

        void GetSummary();


        void SubscribeSession(IUserNotification listener);

        void GetAnalytics();

        UserInfo GetUser();

        SessionInfo GetSessionInfo();


    }
}
