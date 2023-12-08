using MessengerDashboard.Client;

namespace MessengerDashboard.Server.Events
{
    public class SessionUpdatedEventArgs
    {
        public SessionInfo Session { get; set; }

        public SessionUpdatedEventArgs (SessionInfo session)
        {
            Session = session;
        }   
    }
}
