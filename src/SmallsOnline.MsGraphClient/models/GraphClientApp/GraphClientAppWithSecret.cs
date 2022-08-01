namespace SmallsOnline.MsGraphClient.Models;

/// <summary>
/// The confidential client app for authenticating to the Microsoft Graph API when utilizing an application secret.
/// </summary>
/// /// <remarks>
/// Uses <see cref="GraphClientApp" /> as it's base class.
/// </remarks>
public class GraphClientAppWithSecret : GraphClientApp
{
    public GraphClientAppWithSecret(string clientId, string tenantId, string secret, ApiScopesConfig scopesConfig) : base(clientId, tenantId, scopesConfig)
    {
        ConfidentialClientApp = ConfidentialClientApplicationBuilder.Create(ClientId)
            .WithClientSecret(secret)
            .Build();
    }
}