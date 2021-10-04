using Microsoft.Identity.Client;

namespace SmallsOnline.MsGraphClient.Models.Common
{
    public interface IConfidentialClientAppConfig
    {
        string ClientId { get; set; }
        
        string TenantId { get; set; }

        ApiScopesConfig ScopesConfig { get; set; }

        IConfidentialClientApplication ConfidentialClientApp { get; set; }

        AuthenticationResult AuthenticationResult { get; set; }

        void Connect();

        void CreateClientApp();
    }
}