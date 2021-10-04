using System;
using System.Net.Http;

namespace SmallsOnline.MsGraphClient.Models
{
    public interface IGraphClient
    {
        Uri BaseUri { get; set; }

        Boolean IsConnected { get; set; }

        string SendApiCall(string endpoint, string apiPostBody, HttpMethod httpMethod);
    }
}