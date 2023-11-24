/******************************************************************************
* Filename    = MockScreenShareClient.cs
*
* Author      = Pratham Ravindra Nagpure 
*
* Roll number = 112001054
*
* Product     = Messenger 
* 
* Project     = MessengerTests
*
* Description = A class for mocking the screenshare client.
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerScreenshare.Client;

namespace MessengerTests.DashboardTests.Mocks
{
    internal class MockScreenShareClient : IScreenshareClient
    {
        public void SetUser(int id, string name)
        {
        }
    }
}
