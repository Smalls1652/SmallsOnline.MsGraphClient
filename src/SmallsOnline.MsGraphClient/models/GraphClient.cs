using System.Security.Cryptography.X509Certificates;
using SmallsOnline.MsGraphClient.Models.Common;

namespace SmallsOnline.MsGraphClient.Models;


/// <summary>
/// Hosts the client for interacting with the Microsoft Graph API.
/// </summary>
public partial class GraphClient : IGraphClient
{
    public GraphClient(Uri baseUri, string clientId, string tenantId, X509Certificate2 clientCertificate, ApiScopesConfig apiScopes)
    {
        BaseUri = baseUri;
        _httpClient = new();
        _httpClient.BaseAddress = baseUri;
        _httpClient.DefaultRequestHeaders.Add("ConsistencyLevel", "eventual");

        _graphClientApp = new GraphClientAppWithCertificate(clientId, tenantId, clientCertificate, apiScopes);
        _graphClientApp.Connect();

        IsConnected = true;
    }


    public GraphClient(Uri baseUri, string clientId, string tenantId, GraphClientCredentialType credentialType, string clientSecret, ApiScopesConfig apiScopes)
    {
        BaseUri = baseUri;
        _httpClient = new();
        _httpClient.BaseAddress = baseUri;
        _httpClient.DefaultRequestHeaders.Add("ConsistencyLevel", "eventual");

        switch (credentialType)
        {
            case GraphClientCredentialType.CertificateThumbprint:
                X509Certificate2 cert = GetCertificate(clientSecret);

                _graphClientApp = new GraphClientAppWithCertificate(clientId, tenantId, cert, apiScopes);
                _graphClientApp.Connect();
                break;

            default:
                _graphClientApp = new GraphClientAppWithSecret(clientId, tenantId, clientSecret, apiScopes);
                _graphClientApp.Connect();
                break;
        }

        IsConnected = true;
    }

    public Uri BaseUri { get; set; }

    public bool IsConnected { get; set; }

    private readonly IGraphClientApp _graphClientApp;
    private readonly HttpClient _httpClient;
}