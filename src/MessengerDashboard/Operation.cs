using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard
{
    public enum Operation
    {
        EndSession,
        ExamMode,
        LabMode,
        AddClient,
        GetSummary,
        GetTelemetryAnalysis,
        GetSentiment,
        RemoveClient,
        ID,
        AddClientACK
    }
}
