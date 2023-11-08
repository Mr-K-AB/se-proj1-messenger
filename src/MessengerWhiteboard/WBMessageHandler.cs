using System.Diagnostics;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Threading;
using MessengerNetworking.NotificationHandler;

namespace MessengerWhiteboard
{
    public partial class ViewModel : INotificationHandler
    {
        private Dispatcher MainThreadDispatcher => (Application.Current?.Dispatcher != null) ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher;
        public void OnDataReceived(string data)
        {
            Debug.Print("OnDataReceived: {0}", data);
        }

        public void HandleData(string data)
        {
            throw new NotImplementedException();
        }

        public void OnClientJoined(string ipAddr, int port)
        {
            throw new NotImplementedException();
        }

        public void OnClientLeft(string ipAddr, int port)
        {
            throw new NotImplementedException();
        }
    }
}
