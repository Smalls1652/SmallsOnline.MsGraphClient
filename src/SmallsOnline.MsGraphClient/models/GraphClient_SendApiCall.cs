using System.Net;
using System.Net.Http.Headers;

namespace SmallsOnline.MsGraphClient.Models;

public partial class GraphClient : IGraphClient
{
    public string? SendApiCall(string endpoint, string apiPostBody, HttpMethod httpMethod)
    {
        Task<string?> sendApiCallAsyncTask = Task.Run(async () => await SendApiCallAsync(endpoint, apiPostBody, httpMethod));

        return sendApiCallAsyncTask.Result;
    }

    public async Task<string?> SendApiCallAsync(string endpoint, string apiPostBody, HttpMethod httpMethod)
    {
        if (_graphClientApp.AuthenticationResult is null)
        {
            throw new Exception("AuthenticationResult is null. The GraphClientApp was potentially not connected.");
        }

        string? apiResponse = null;

        CheckAuthStatus();

        HttpRequestMessage requestMessage = new(httpMethod, endpoint);
        requestMessage.Headers.Authorization = new("Bearer", _graphClientApp.AuthenticationResult.AccessToken);

        switch (string.IsNullOrEmpty(apiPostBody))
        {
            case false:
                requestMessage.Content = new StringContent(apiPostBody);
                requestMessage.Content.Headers.ContentType = new("application/json");
                break;

            default:
                break;
        }

        bool isFinished = false;

        while (!isFinished)
        {
            HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);

            switch (responseMessage.StatusCode)
            {
                case HttpStatusCode.TooManyRequests:
                    RetryConditionHeaderValue? retryAfterValue = responseMessage.Headers.RetryAfter;

                    TimeSpan retryAfterBuffer;
                    if (retryAfterValue is null || retryAfterValue.Delta is null)
                    {
                        retryAfterBuffer = new(
                            hours: 0,
                            minutes: 0,
                            seconds: 30
                        );
                    }
                    else
                    {
                        retryAfterBuffer = retryAfterValue.Delta.Value.Add(TimeSpan.FromSeconds(15));
                    }

                    Console.WriteLine($"--- !!! Throttling for: {retryAfterBuffer.TotalSeconds} seconds !!! ---");
                    Thread.Sleep(retryAfterBuffer);
                    break;

                default:
                    isFinished = true;

                    apiResponse = await responseMessage.Content.ReadAsStringAsync();
                    responseMessage.Dispose();
                    break;
            }
        }

        requestMessage.Dispose();

        return apiResponse;
    }
}