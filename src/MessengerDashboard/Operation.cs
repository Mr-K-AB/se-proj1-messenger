using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard
{
    public enum Operation
    {
        EndSession,
        ToggleSessionMode,
        AddClient,
        GetSummary,
        GetAnalytics,
        RemoveClient,
        ID,
        AddClientACK
    }
}
