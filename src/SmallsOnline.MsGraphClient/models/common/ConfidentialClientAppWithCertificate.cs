using System.Security.Cryptography.X509Certificates;

namespace SmallsOnline.MsGraphClient.Models.Common
{
    /// <summary>
    /// The confidential client app for authenticating to the Microsoft Graph API when utilizing a certificate.
    /// </summary>
    public class ConfidentialClientAppWithCertificate : IConfidentialClientAppConfig
    {
        public ConfidentialClientAppWithCertificate() {}

        public ConfidentialClientAppWithCertificate(string clientId, string tenantId, X509Certificate2 clientCert, ApiScopesConfig scopesConfig)
        {
            ClientId = clientId;
            TenantId = tenantId;
            ScopesConfig = scopesConfig;
            clientCertificate = clientCert;
        }

        public string ClientId { get; set; }

        public string TenantId { get; set; }

        public ApiScopesConfig ScopesConfig { get; set; }

        private protected X509Certificate2 clientCertificate;

        public IConfidentialClientApplication ConfidentialClientApp { get; set; }

        public AuthenticationResult AuthenticationResult { get; set; }

        public void Connect()
        {
            if (null == this.ConfidentialClientApp)
            {
                CreateClientApp();
            }

            Task<AuthenticationResult> getTokenTask = Task<AuthenticationResult>.Run(
                async () => await GetTokenForClientAsync(ScopesConfig.Scopes)
            );

            Task.WaitAll(getTokenTask);

            AuthenticationResult = getTokenTask.Result;
        }

        public void CreateClientApp()
        {
            ConfidentialClientApp = ConfidentialClientApplicationBuilder.Create(this.ClientId)
                .WithCertificate(this.clientCertificate)
                .Build();

            this.clientCertificate = null;
        }

        private async Task<AuthenticationResult> GetTokenForClientAsync(IEnumerable<string> scopes)
        {
            AuthenticationResult authResult = null;

            authResult = await this.ConfidentialClientApp.AcquireTokenForClient(scopes)
                .WithAuthority(AzureCloudInstance.AzurePublic, this.TenantId)
                .ExecuteAsync();

            return authResult;
        }
    }
}