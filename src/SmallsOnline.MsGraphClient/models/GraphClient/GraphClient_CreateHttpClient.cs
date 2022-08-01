namespace SmallsOnline.MsGraphClient.Models;

public partial class GraphClient : IGraphClient
{
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