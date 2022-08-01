namespace SmallsOnline.MsGraphClient.Models;

public partial class GraphClient : IGraphClient
{
    private void CheckAuthStatus()
    {
        DateTimeOffset currentDateTime = DateTimeOffset.Now;
        if (currentDateTime >= _graphClientApp.AuthenticationResult.ExpiresOn)
        {
            _graphClientApp.Connect();
        }
    }
}