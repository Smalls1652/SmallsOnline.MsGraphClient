namespace SmallsOnline.MsGraphClient.Models;

public partial class GraphClient : IGraphClient
{
    /// <summary>
    /// Create a HTTP client to handle API calls.
    /// </summary>
    /// <param name="baseUri">The base URI of the API.</param>
    /// <returns>A <see cref="HttpClient" /> instance.</returns>
    private static HttpClient CreateHttpClient(Uri baseUri)
    {
        HttpClient httpClient = new()
        {
            BaseAddress = baseUri
        };
        httpClient.DefaultRequestHeaders.Add("ConsistencyLevel", "eventual");

        return httpClient;
    }
}