namespace SmallsOnline.MsGraphClient.Models;

/// <summary>
/// A collection of scopes to use when authenticating to the Microsoft Graph API.
/// </summary>
public class ApiScopesConfig
{
    public ApiScopesConfig(IEnumerable<string> scopes)
    {
        Scopes = scopes;
    }

    public IEnumerable<string> Scopes { get; set; }
}