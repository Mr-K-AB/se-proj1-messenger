/// <credits>
/// <author>
/// <name>Shailab Chauhan</name>
/// <rollnumber>112001038</rollnumber>
/// </author>
/// </credits>

using System;

namespace MessengerDashboard.Client
{
    public class ClientInfo
    {
        public ClientInfo()
        {

        }

        public ClientInfo(
            string clientName,
            int clientID,
            string? clientEmail = null,
            string? clientPhotoUrl = null
        )
        {
            ClientId = clientID;
            ClientName = clientName;
            ClientEmail = clientEmail;
            ClientPhotoUrl = clientPhotoUrl;
        }

        public string? ClientEmail { get; set; }

        public int ClientId { get; set; }

        public string? ClientName { get; set; }

        public string? ClientPhotoUrl { get; set; }

        public bool Equals(ClientInfo client)
        {
            if (client == null)
            {
                return false;
            }

            return ClientId.Equals(client.ClientId) &&
                   (ReferenceEquals(ClientName, client.ClientName) ||
                    ClientName != null && ClientName.Equals(client.ClientName));
        }
    }
}

