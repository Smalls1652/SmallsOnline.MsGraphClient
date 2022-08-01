namespace SmallsOnline.MsGraphClient.Models.Common;

/// <summary>
/// The confidential client app for authenticating to the Microsoft Graph API when utilizing an application secret.
/// </summary>
public class ConfidentialClientAppWithSecret : ConfidentialClientAppBase
{
    public ConfidentialClientAppWithSecret(string clientId, string tenantId, string secret, ApiScopesConfig scopesConfig)
    {
        ClientId = clientId;
        TenantId = tenantId;
        ScopesConfig = scopesConfig;

        ConfidentialClientApp = ConfidentialClientApplicationBuilder.Create(ClientId)
            .WithClientSecret(secret)
            .Build();
    }
}