namespace SmallsOnline.MsGraphClient.Models;

public partial class GraphClient : IGraphClient
{
    /// <inheritdoc cref="ConnectClientAsync" />
    /// <remarks>
    /// This calls <see cref="ConnectClientAsync" />.
    /// </remarks>
    public void ConnectClient()
    {
        Task connectClientTask = Task.Run(async () => await ConnectClientAsync());

        connectClientTask.Wait();
    }

    /// <summary>
    /// Connect the Graph Client.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public async Task ConnectClientAsync()
    {
        try
        {
            await _graphClientApp.ConnectAsync();
            _isConnected = true;
        }
        catch
        {
            throw new Exception("An error occurred while connecting the client app.");
        }
    }
}