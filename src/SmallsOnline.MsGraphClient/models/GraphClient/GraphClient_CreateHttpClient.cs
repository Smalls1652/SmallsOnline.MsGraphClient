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
        // Create the HttpClient and set the BaseAddress property to
        // the provided URI.
        HttpClient httpClient = new()
        {
            BaseAddress = baseUri
        };

        // Add a default request header to set
        // the 'ConsistencyLevel' to 'eventual'.
        httpClient.DefaultRequestHeaders.Add("ConsistencyLevel", "eventual");

        // Return the created HttpClient.
        return httpClient;
    }
}