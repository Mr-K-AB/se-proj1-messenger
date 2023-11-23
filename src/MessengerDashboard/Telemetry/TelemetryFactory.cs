/******************************************************************************
* Filename    = TelemetryFactory.cs
*
* Author      = Aradhya Bijalwan
*
* Product     = MessengerApp
* 
* Project     = MessengerDashboard
*
* Description = Factory class providing a method to obtain a new instance of TelemetryManager.
*****************************************************************************/
using System;
using System.Collections.Generic;
namespace MessengerDashboard.Telemetry

{   
    /// <summary>
    /// Factory class responsible for creating instances of telemetry-related components.
    /// </summary>
    public class TelemetryFactory
    {  
        /// <summary>
        /// Returns a new instance of the ITelemetry interface using TelemetryManager.
        /// </summary>
        /// <returns>A new instance of the ITelemetry interface.</returns>
        public static ITelemetry GetTelemetryInstance()
        {
            return new TelemetryManager();
        }
    }
}



