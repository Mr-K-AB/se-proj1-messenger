using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Client
{
    public interface IClientSessionController
    {

        bool AddUser();

        void ToggleSessionType();

        void RemoveUser();

        void EndMeeting();

        void GetSummary();

        void GetAnalytics();

        UserInfo GetUser();

        SessionInfo GetSessionInfo();

    }
}
