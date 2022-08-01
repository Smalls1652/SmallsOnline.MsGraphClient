namespace SmallsOnline.MsGraphClient.Models;

public partial class GraphClient : IGraphClient
{
    /// <summary>
    /// Checks the authentication status of the GraphClient. If the token has expired, it will refresh it.
    /// </summary>
    /// <exception cref="Exception"></exception>
    private void CheckAuthStatus()
    {
        if (_graphClientApp.AuthenticationResult is null)
        {
            throw new Exception("AuthenticationResult is null. The GraphClientApp was potentially not connected.");
        }

        // Get the current date/time.
        // If it's greater than the expiration date/time, then refresh the token.
        DateTimeOffset currentDateTime = DateTimeOffset.Now;
        if (currentDateTime >= _graphClientApp.AuthenticationResult.ExpiresOn)
        {
            _graphClientApp.Connect();
        }
    }
}