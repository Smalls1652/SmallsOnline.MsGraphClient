namespace SmallsOnline.MsGraphClient.Models.Common;

public abstract class ConfidentialClientAppBase : IConfidentialClientApp
{
    public string ClientId { get; set; }

    public string TenantId { get; set; }

    public ApiScopesConfig ScopesConfig { get; set; }

    public IConfidentialClientApplication ConfidentialClientApp { get; set; }

    public AuthenticationResult AuthenticationResult { get; set; }

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
        AuthenticationResult? authResult;

        authResult = await ConfidentialClientApp.AcquireTokenForClient(scopes)
            .WithAuthority(AzureCloudInstance.AzurePublic, TenantId)
            .ExecuteAsync();

        return authResult;
    }
}