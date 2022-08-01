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
        httpClient = new();
        httpClient.BaseAddress = baseUri;
        httpClient.DefaultRequestHeaders.Add("ConsistencyLevel", "eventual");

        ConfidentialClientApp = new ConfidentialClientAppWithCertificate(clientId, tenantId, clientCertificate, apiScopes);
        ConfidentialClientApp.Connect();

        IsConnected = true;
    }


    public GraphClient(Uri baseUri, string clientId, string tenantId, GraphClientCredentialType credentialType, string clientSecret, ApiScopesConfig apiScopes)
    {
        BaseUri = baseUri;
        httpClient = new();
        httpClient.BaseAddress = baseUri;
        httpClient.DefaultRequestHeaders.Add("ConsistencyLevel", "eventual");

        switch (credentialType)
        {
            case GraphClientCredentialType.CertificateThumbprint:
                X509Certificate2 cert = GetCertificate(clientSecret);

                ConfidentialClientApp = new ConfidentialClientAppWithCertificate(clientId, tenantId, cert, apiScopes);
                ConfidentialClientApp.Connect();
                break;

            default:
                ConfidentialClientApp = new ConfidentialClientAppWithSecret(clientId, tenantId, clientSecret, apiScopes);
                ConfidentialClientApp.Connect();
                break;
        }

        IsConnected = true;
    }

    public Uri BaseUri { get; set; }

    public bool IsConnected { get; set; }

    private readonly IGraphClientAppConfig ConfidentialClientApp;

    private static HttpClient httpClient;
}