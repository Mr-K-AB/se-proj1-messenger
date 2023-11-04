using System;
using System.Collections.Generic;
namespace MessengerDashboard.Telemetry

{
    public class TelemetryFactory
    {
        public static ITelemetry GetTelemetryInstance()
        {
            return new TelemetryManager();
        }
    }
}



