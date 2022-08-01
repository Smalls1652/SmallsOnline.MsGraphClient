using System.Net;
using System.Net.Http.Headers;

namespace SmallsOnline.MsGraphClient.Models;

public partial class GraphClient : IGraphClient
{
    /// <inheritdoc cref="SendApiCallAsync" />
    /// /// <remarks>
    /// This calls <see cref="SendApiCallAsync" />.
    /// </remarks>
    public string? SendApiCall(string endpoint, string apiPostBody, HttpMethod httpMethod)
    {
        Task<string?> sendApiCallAsyncTask = Task.Run(async () => await SendApiCallAsync(endpoint, apiPostBody, httpMethod));

        return sendApiCallAsyncTask.Result;
    }

    /// <summary>
    /// Send an API call to Microsoft Graph.
    /// </summary>
    /// <param name="endpoint">The endpoint to send the API call to.</param>
    /// <param name="apiPostBody">Content to be sent to the API, if needed.</param>
    /// <param name="httpMethod">The HTTP method to use for sending the API call.</param>
    /// <returns>Data returned by the API, if any was returned.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<string?> SendApiCallAsync(string endpoint, string apiPostBody, HttpMethod httpMethod)
    {
        if (_isConnected == false)
        {
            throw new Exception("The Graph client app has not been connected initially.");
        }

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