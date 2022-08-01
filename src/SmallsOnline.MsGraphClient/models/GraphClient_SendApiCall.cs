using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SmallsOnline.MsGraphClient.Models;

public partial class GraphClient : IGraphClient
{
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
}