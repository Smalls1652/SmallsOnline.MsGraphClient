namespace SmallsOnline.MsGraphClient.Models.Common;

public abstract class GraphClientApp : IGraphClientApp
{
    public string ClientId { get; set; } = null!;

    public string TenantId { get; set; } = null!;

    public ApiScopesConfig ScopesConfig { get; set; } = null!;

    public IConfidentialClientApplication? ConfidentialClientApp { get; set; }

    public AuthenticationResult? AuthenticationResult { get; set; }

    public void Connect()
    {
        Task connectAsyncTask = Task.Run(async () => await ConnectAsync());

        connectAsyncTask.Wait();
    }

    public async Task ConnectAsync()
    {
        AuthenticationResult = await GetTokenForClientAsync(ScopesConfig.Scopes);
    }

    private async Task<AuthenticationResult> GetTokenForClientAsync(IEnumerable<string> scopes)
    {
        if (ConfidentialClientApp is not null)
        {
            AuthenticationResult? authResult;

            authResult = await ConfidentialClientApp.AcquireTokenForClient(scopes)
                .WithAuthority(AzureCloudInstance.AzurePublic, TenantId)
                .ExecuteAsync();

            return authResult;
        }
        else
        {
            throw new NullReferenceException("The ConfidentialClientApp property is null.");
        }
    }
}