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
    public Dictionary<string , int> UserChatCount = new Dictionary<string , int>();//
    public Dictionary<stirng, int> userIdVsEmailId = new Dictionary<stiring, int>();//email store
    // to store the start time of session
    private DateTime sessíonStarttime;

    public Telemetry()
    {
        sessionStartime = DateTime.Now;
        serverSessionManager.Subscribe(this);

    }
    public void UpdateUserNameVsChatCount()
    {   
        userNameVsChatCount.Clear();  
        foreach (var currUserChatCount in userIdVsChatCount)
        {

            string currEmailId = userIdVsEmailId[currUserChatCount.Key];
            string currUserName = emailIdVsUserName[currEmailId];
            if (userNameVsChatCount.ContainsKey(currUserName) == false)
            {
                userNameVsChatCount[currUserName] = 0 + currUserChatCount.Value;

            }
            else
            {
                userNameVsChatCount[currUserName] = userNameVsChatCount[currUserName] + currUserChatCount.Value;

            }

        }

        return;
    }
    // TODO:
    public SessionAnalytics GetTelemetryAnalytics(object all)//this will get a thread for all which will get our data from different obejct
    {


    }
    public void SaveAnalytics()
    {

    }

    public class adduser
    {

    }


}
