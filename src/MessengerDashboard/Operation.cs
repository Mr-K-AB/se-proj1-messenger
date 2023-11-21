namespace MessengerDashboard
{
    public enum Operation
    {
        GiveUserDetails, /* Server assigns id to client and requests client for user details */
        TakeUserDetails, /* Client requests server to take the user details */
        Refresh,         /* Client asks server for sending new data. Server replies by new data  */
        EndSession,      /* Server tells everyone that session has ended */
        ExamMode,        /* Client requests server to set exam mode only instructor can do this */
        LabMode,         /* Client requests server to set exam mode only instructor can do this */
        RemoveClient,    /* Client requests server to remove client, server replies by removing client */
        SessionUpdated,  /* Server tells everyone that session has updated */
    }
}
