using System;

namespace SmallsOnline.MsGraphClient.Models;

public partial class GraphClient : IGraphClient
{
    private void CheckAuthStatus()
    {
        DateTimeOffset currentDateTime = DateTimeOffset.Now;
        if (currentDateTime >= ConfidentialClientApp.AuthenticationResult.ExpiresOn)
        {
            ConfidentialClientApp.Connect();
        }
    }
}