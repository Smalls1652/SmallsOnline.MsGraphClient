namespace SmallsOnline.MsGraphClient.Models;

public partial class GraphClient : IGraphClient
{
    private void CheckAuthStatus()
    {
        if (_graphClientApp.AuthenticationResult is null)
        {
            throw new Exception("AuthenticationResult is null. The GraphClientApp was potentially not connected.");
        }

        DateTimeOffset currentDateTime = DateTimeOffset.Now;
        if (currentDateTime >= _graphClientApp.AuthenticationResult.ExpiresOn)
        {
            _graphClientApp.Connect();
        }
    }
}