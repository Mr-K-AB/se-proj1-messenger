/******************************************************************************
* Filename    = DispatchUtils.cs
*
* Author      = M V Nagasurya
*
* Product     = MessengerApp
* 
* Project     = MessengerTests
*
* Description = Process the Pending messages and events present in the message queue of the UI thread
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MessengerTests.ContentTests
{
    public class DispatchUtils
    {
        public static class UiDispatcherHelper
        {
            /// <summary>
            /// Stop the frame 
            /// </summary>
            /// <param name="frame"></param>
            /// <returns></returns>
            private static object? ExitFrame(object frame)
            {
                ((DispatcherFrame)frame).Continue = false;
                return null;
            }

            /// <summary>
            /// Asynchronously invokes the ExitFrame method to set up a synchronous UI event processing loop,
            /// and pushes the frame onto the Dispatcher stack to ensure immediate processing of UI-related operations
            /// in unitTests.
            /// </summary>
            public static void ProcessUiEvents()
            {
                var frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(ExitFrame), frame);
                Dispatcher.PushFrame(frame);
            }
        }
    }
}
