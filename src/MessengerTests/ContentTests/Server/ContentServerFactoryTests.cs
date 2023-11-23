/******************************************************************************
 * Filename    = ContentServerFactoryTests.cs
 *
 * Author      = Manikanta Gudipudi
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContentTests
 *
 * Description = Tests for ContentServerFactory
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerContent.Server;

namespace MessengerTests.ContentTests.Server
{
    [TestClass]
    public class ContentServerFactoryTest
    {
        [TestMethod]
        public void GetInstance_SingleThread_ReturnsSingleInstance()
        {
            IContentServer ref1 = ContentServerFactory.GetInstance();
            IContentServer ref2 = ContentServerFactory.GetInstance();

            Assert.AreSame(ref1, ref2);
        }
    }
}
