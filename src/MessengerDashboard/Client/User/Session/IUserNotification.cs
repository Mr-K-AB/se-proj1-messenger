using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard;
using MessengerDashboard.Dashboard;

namespace MessengerDashboard.Dashboard.User.Session
{
    public interface IUserNotification
    {
        void OnUserSessionChange( SessionInfo session );
    }
}
