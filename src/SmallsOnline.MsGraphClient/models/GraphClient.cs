using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

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
            httpClient = new();
            httpClient.BaseAddress = baseUri;
            httpClient.DefaultRequestHeaders.Add("ConsistencyLevel", "eventual");

            ConfidentialClientApp = new ConfidentialClientAppWithCertificate(clientId, tenantId, clientCertificate, apiScopes);
            ConfidentialClientApp.Connect();

            IsConnected = true;
        }


        public GraphClient(Uri baseUri, string clientId, string tenantId, GraphClientCredentialType credentialType, string clientSecret, ApiScopesConfig apiScopes)
        {
            BaseUri = baseUri;
            httpClient = new();
            httpClient.BaseAddress = baseUri;
            httpClient.DefaultRequestHeaders.Add("ConsistencyLevel", "eventual");

            switch (credentialType)
            {
                case GraphClientCredentialType.CertificateThumbprint:
                    X509Certificate2 cert = GetCertificate(clientSecret);

                    ConfidentialClientApp = new ConfidentialClientAppWithCertificate(clientId, tenantId, cert, apiScopes);
                    ConfidentialClientApp.Connect();
                    break;

                default:
                    ConfidentialClientApp = new ConfidentialClientAppWithSecret(clientId, tenantId, clientSecret, apiScopes);
                    ConfidentialClientApp.Connect();
                    break;
            }

            IsConnected = true;
        }

        public Uri BaseUri { get; set; }

        public bool IsConnected { get; set; }

        private readonly IConfidentialClientAppConfig ConfidentialClientApp;

        private static HttpClient httpClient;

        public string SendApiCall(string endpoint, string apiPostBody, HttpMethod httpMethod)
        {
            string apiResponse = null;

            DateTimeOffset currentDateTime = DateTimeOffset.Now;
            if (currentDateTime >= ConfidentialClientApp.AuthenticationResult.ExpiresOn)
            {
                ConfidentialClientApp.Connect();
            }

            HttpRequestMessage requestMessage = new(httpMethod, endpoint);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.ConfidentialClientApp.AuthenticationResult.AccessToken);

            switch (String.IsNullOrEmpty(apiPostBody))
            {
                case false:
                    requestMessage.Content = new StringContent(apiPostBody);
                    requestMessage.Content.Headers.ContentType = new("application/json");
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

                        Task<string> apiResponseReadTask = Task.Run(
                            async () =>
                            {
                                string response = await responseMessage.Content.ReadAsStringAsync();

                                return response;
                            }
                        );

                        apiResponseReadTask.Wait();
                        apiResponse = apiResponseReadTask.Result;

                        apiResponseReadTask.Dispose();
                        responseMessage.Dispose();
                        break;
                }
            }

            requestMessage.Dispose();

            return apiResponse;
        }

        private static X509Certificate2 GetCertificate(string thumprint)
        {
            X509Certificate2 cert;

            using (X509Store certStore = new(StoreLocation.LocalMachine))
            {
                try
                {
                    certStore.Open(OpenFlags.ReadOnly);

                    X509Certificate2Collection certsInStore = certStore.Certificates;
                    X509Certificate2Collection foundCerts = certsInStore.Find(X509FindType.FindByThumbprint, thumprint, false);

                    if (foundCerts.Count != 1)
                    {
                        throw new Exception("Too many certificates were returned with the same thumbprint or no certificates were found.");
                    }

                    cert = foundCerts[0];
                }
                finally
                {
                    certStore.Close();
                }
            }

            return cert;
        }
    }
}