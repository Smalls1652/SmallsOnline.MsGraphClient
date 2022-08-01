namespace SmallsOnline.MsGraphClient.Models;

/// <summary>
/// The base class for configuring and connecting a <see cref="GraphClient" />.
/// </summary>
/// <remarks>
/// This class should not be used by itself. It should only be used to create other classes that should inherit it's base properties and methods.
/// </remarks>
public abstract class GraphClientApp : IGraphClientApp
{
    public GraphClientApp(string clientId, string tenantId, ApiScopesConfig scopesConfig)
    {
        ClientId = clientId;
        TenantId = tenantId;
        ScopesConfig = scopesConfig;
    }

    /// <summary>
    /// The client ID for the app.
    /// </summary>
    public string ClientId { get; set; } = null!;

    /// <summary>
    /// The tenant ID that the app is registered to.
    /// </summary>
    public string TenantId { get; set; } = null!;

    /// <inheritdoc cref="ApiScopesConfig" />
    public ApiScopesConfig ScopesConfig { get; set; } = null!;

    /// <summary>
    /// The <see cref="IConfidentialClientApplication" /> generated to use for authenticating to Azure AD.
    /// </summary>
    public IConfidentialClientApplication? ConfidentialClientApp { get; set; }

    /// <inheritdoc cref="Microsoft.Identity.Client.AuthenticationResult" />
    public AuthenticationResult? AuthenticationResult { get; set; }

    /// <inheritdoc cref="ConnectAsync" />
    /// <remarks>
    /// Calls <see cref="ConnectAsync" />.
    /// </remarks>
    public void Connect()
    {
        Task connectAsyncTask = Task.Run(async () => await ConnectAsync());

        connectAsyncTask.Wait();
    }

    /// <summary>
    /// Connect the client app by getting an authentication token from Azure AD.
    /// </summary>
    public async Task ConnectAsync()
    {
        AuthenticationResult = await GetTokenForClientAsync(ScopesConfig.Scopes);
    }

    /// <summary>
    /// Get an authentication token for the client.
    /// </summary>
    /// <param name="scopes">A collection of scopes to use from the API.</param>
    /// <returns>An <see cref="Microsoft.Identity.Client.AuthenticationResult" /> object.</returns>
    /// <exception cref="NullReferenceException"></exception>
    private async Task<AuthenticationResult> GetTokenForClientAsync(IEnumerable<string> scopes)
    {
        if (ConfidentialClientApp is not null)
        {
            // If the ConfidentialClientApp property is not null,
            // get an authentication token for the client app.
            AuthenticationResult? authResult;

            authResult = await ConfidentialClientApp.AcquireTokenForClient(scopes)
                .WithAuthority(AzureCloudInstance.AzurePublic, TenantId)
                .ExecuteAsync();

            return authResult;
        }
        else
        {
            // Otherwise, throw an error.
            // Can't call the necessary method for acquiring a token if that's null, now can we?
            throw new NullReferenceException("The ConfidentialClientApp property is null.");
        }
    }
}