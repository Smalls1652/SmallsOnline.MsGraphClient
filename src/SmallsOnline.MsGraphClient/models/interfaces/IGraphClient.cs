namespace SmallsOnline.MsGraphClient.Models
{
    public interface IGraphClient
    {
        bool IsConnected { get; set; }

        string? SendApiCall(string endpoint, string apiPostBody, HttpMethod httpMethod);
        Task<string?> SendApiCallAsync(string endpoint, string apiPostBody, HttpMethod httpMethod);
    }
}