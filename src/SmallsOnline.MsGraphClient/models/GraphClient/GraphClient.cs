using System.Security.Cryptography.X509Certificates;

namespace SmallsOnline.MsGraphClient.Models;


/// <summary>
/// Hosts the client for interacting with the Microsoft Graph API.
/// </summary>
public partial class GraphClient : IGraphClient
{
    public GraphClient(Uri baseUri, string clientId, string tenantId, X509Certificate2 clientCertificate, ApiScopesConfig apiScopes)
    {
        _httpClient = CreateHttpClient(baseUri);

        _graphClientApp = new GraphClientAppWithCertificate(clientId, tenantId, clientCertificate, apiScopes);
    }


    public GraphClient(Uri baseUri, string clientId, string tenantId, GraphClientCredentialType credentialType, string clientSecret, ApiScopesConfig apiScopes)
    {
        _httpClient = CreateHttpClient(baseUri);

        // Evaluate how to create the GraphClientApp based off the credential type.
        switch (credentialType)
        {
            // If it's a certificate thumbprint
            case GraphClientCredentialType.CertificateThumbprint:
                X509Certificate2 cert = GetCertificate(clientSecret);

                _graphClientApp = new GraphClientAppWithCertificate(clientId, tenantId, cert, apiScopes);
                break;

            // Otherwise, it's a secret.
            default:
                _graphClientApp = new GraphClientAppWithSecret(clientId, tenantId, clientSecret, apiScopes);
                break;
        }
    }

    /// <summary>
    /// Whether the GraphClient has been connected or not.
    /// </summary>
    public bool IsConnected
    {
        get => _isConnected;
    }
    private bool _isConnected = false;

    private readonly IGraphClientApp _graphClientApp;
    private readonly HttpClient _httpClient;
}