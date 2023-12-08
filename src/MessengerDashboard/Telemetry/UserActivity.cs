/******************************************************************************
* Filename    = UserActivity.cs
*
* Author      = Aradhya Bijalwan
*
* Roll Number = 112001006
*
* Product     = MessengerApp
* 
* Project     = MessengerDashboard
*
* Description = model class for user related data
*****************************************************************************/
using System;

namespace MessengerDashboard.Telemetry
{
    public class UserActivity
    {
        public int UserChatCount { get; set; }

        public string? UserName { get; set; }

        public string? UserEmail { get; set; }

        public DateTime EntryTime { get; set; }

        public DateTime ExitTime { get; set; } = DateTime.MinValue;
    }
}
