using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Identity.Client;

namespace SmallsOnline.MsGraphClient.Models
{
    using SmallsOnline.MsGraphClient.Models.Common;

    /// <summary>
    /// Hosts the client for interacting with the Microsoft Graph API.
    /// </summary>
    public class GraphClient : IGraphClient
    {
        public GraphClient(Uri baseUri, string clientId, string tenantId, X509Certificate2 clientCertificate, ApiScopesConfig apiScopes)
        {
            BaseUri = baseUri;
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseUri;
            httpClient.DefaultRequestHeaders.Add("ConsistencyLevel", "eventual");

            ConfidentialClientApp = new ConfidentialClientAppWithCertificate(clientId, tenantId, clientCertificate, apiScopes);
            ConfidentialClientApp.Connect();

            IsConnected = true;
        }

        public GraphClient(Uri baseUri, string clientId, string tenantId, string clientSecret, ApiScopesConfig apiScopes)
        {
            BaseUri = baseUri;
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseUri;
            httpClient.DefaultRequestHeaders.Add("ConsistencyLevel", "eventual");

            ConfidentialClientApp = new ConfidentialClientAppWithSecret(clientId, tenantId, clientSecret, apiScopes);
            ConfidentialClientApp.Connect();

            IsConnected = true;
        }

        public Uri BaseUri { get; set; }

        public bool IsConnected { get; set; }

        private IConfidentialClientAppConfig ConfidentialClientApp;

        private static HttpClient httpClient;

        public string SendApiCall(string endpoint, string apiPostBody, HttpMethod httpMethod)
        {
            string apiResponse = null;

            DateTimeOffset currentDateTime = DateTimeOffset.Now;
            if (currentDateTime >= ConfidentialClientApp.AuthenticationResult.ExpiresOn)
            {
                ConfidentialClientApp.Connect();
            }

            HttpRequestMessage requestMessage = new HttpRequestMessage(httpMethod, endpoint);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.ConfidentialClientApp.AuthenticationResult.AccessToken);

            switch (String.IsNullOrEmpty(apiPostBody))
            {
                case false:
                    requestMessage.Content = new StringContent(apiPostBody);
                    requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    break;

                default:
                    break;
            }

            bool isFinished = false;
            //HttpResponseMessage responseMessage;

            while (!isFinished)
            {
                HttpResponseMessage responseMessage = httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();

                switch (responseMessage.StatusCode)
                {
                    case HttpStatusCode.TooManyRequests:
                        RetryConditionHeaderValue retryAfterValue = responseMessage.Headers.RetryAfter;
                        TimeSpan retryAfterBuffer = retryAfterValue.Delta.Value.Add(TimeSpan.FromSeconds(15));

                        Console.WriteLine($"--- !!! Throttling for: {retryAfterBuffer.TotalSeconds} seconds !!! ---");
                        Thread.Sleep(retryAfterBuffer);
                        break;

                    default:
                        isFinished = true;

                        apiResponse = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        responseMessage.Dispose();
                        break;
                }
            }

            requestMessage.Dispose();

            return apiResponse;
        }
    }
}