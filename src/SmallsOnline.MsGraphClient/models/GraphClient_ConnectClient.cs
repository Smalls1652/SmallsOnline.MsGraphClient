namespace SmallsOnline.MsGraphClient.Models;

public partial class GraphClient : IGraphClient
{
    public void ConnectClient()
    {
        Task connectClientTask = Task.Run(async () => await ConnectClientAsync());

        connectClientTask.Wait();
    }

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