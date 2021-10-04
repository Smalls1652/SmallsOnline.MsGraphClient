using System.Collections.Generic;

namespace SmallsOnline.MsGraphClient.Models.Common
{
    /// <summary>
    /// A collection of scopes to use when authenticating to the Microsoft Graph API.
    /// </summary>
    public class ApiScopesConfig
    {
        public IEnumerable<string> Scopes { get; set; }
    }
}