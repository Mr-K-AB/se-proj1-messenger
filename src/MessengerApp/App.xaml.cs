using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MessengerNetworking.Communicator;
using TraceLogger;

namespace MessengerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Logger.Warn(DateTime.Now.ToString("s") +"Application EXIT");
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
