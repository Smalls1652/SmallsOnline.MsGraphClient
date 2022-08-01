using System.Security.Cryptography.X509Certificates;

namespace SmallsOnline.MsGraphClient.Models.Common;

/// <summary>
/// The Graph client app for authenticating to the Microsoft Graph API when utilizing a certificate.
/// </summary>
public class GraphClientAppWithCertificate : GraphClientApp
{
    public GraphClientAppWithCertificate(string clientId, string tenantId, X509Certificate2 clientCert, ApiScopesConfig scopesConfig)
    {
        ClientId = clientId;
        TenantId = tenantId;
        ScopesConfig = scopesConfig;

        ConfidentialClientApp = ConfidentialClientApplicationBuilder.Create(ClientId)
            .WithCertificate(clientCert)
            .Build();
    }
}