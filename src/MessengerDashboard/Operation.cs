namespace MessengerDashboard
{
    public enum Operation
    {
        AddClient, /* Request to server for adding client */ 
        AddClientAcknowledgement, /* Acknowledgement by server for adding client */
        AddClientConfirmation, /* Confirmation by client for getting added */
        Refresh,
        RefreshAcknowledgement,
        EndSession,
        ExamMode, 
        LabMode,
        RemoveClient,
        SessionUpdated,
    }
}
