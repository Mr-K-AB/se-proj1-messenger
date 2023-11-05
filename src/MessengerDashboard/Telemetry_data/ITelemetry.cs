using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Telemetry
{
    pulblic interface ITelemetry
    {
        public void SaveAnalytics();
        public void SessAnalytics GetTelemetryAnalytics();
    }
}
