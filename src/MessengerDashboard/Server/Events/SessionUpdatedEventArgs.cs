using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Client;

namespace MessengerDashboard.Server.Events
{
    public class SessionUpdatedEventArgs
    {
        public SessionInfo Session { get; set; }
    }
}
