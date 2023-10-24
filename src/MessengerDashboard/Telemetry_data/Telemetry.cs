using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Telemetry
{    
    /// <summary>
    /// Class to handle user data
    /// </summary>
    /// 

    // TODO:

    public Dictionary<string, int> UserEnterTime = new Dictionary<string, int>();
    public Dictionary<string , int> UserExitTime = new Dictionary<string , int>();
    public Dictionary<string , int> UserChatCount = new Dictionary<string , int>();
    // to store the start time of session
    private DateTime sessíonStarttime;

    public Telemetry()
    {
        sessionStartime = DateTime.Now;

    }
    // TODO:
    public SessionAnalytics GetTelemetryAnalytics()
    {

    }
    public void SaveAnalytics()
    {

    }

    public class adduser
    {

    }


}
