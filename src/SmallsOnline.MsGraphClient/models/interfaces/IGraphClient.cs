namespace SmallsOnline.MsGraphClient.Models
{
    public interface IGraphClient
    {
        Uri BaseUri { get; set; }

        bool IsConnected { get; set; }

        string SendApiCall(string endpoint, string apiPostBody, HttpMethod httpMethod);
    }
}