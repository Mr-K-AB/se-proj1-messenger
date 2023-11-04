using System;
using System.Collections.Generic;
namespace MessengerDashboard.Telemetry

{
    public class TelemetryFactory
    {
        //this is the factory for the telmetry module 
        //we will be using the singleton design pattern 
        private static readonly Telemetry telemetry;

        public static ITelemetry GetTelemetryInstance()
        {
            return new Telemetry();
        }
    }
}



