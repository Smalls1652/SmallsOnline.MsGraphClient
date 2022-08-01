using System.Net;
using System.Net.Http.Headers;

namespace SmallsOnline.MsGraphClient.Models;

public partial class GraphClient : IGraphClient
{
    /// <inheritdoc cref="SendApiCallAsync" />
    /// /// <remarks>
    /// This calls <see cref="SendApiCallAsync" />.
    /// </remarks>
    public string? SendApiCall(string endpoint, string? apiPostBody, HttpMethod httpMethod)
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
    public async Task<string?> SendApiCallAsync(string endpoint, string? apiPostBody, HttpMethod httpMethod)
    {
        // If the client hasn't been intially connected, throw an error.
        if (_isConnected == false)
        {
            throw new Exception("The Graph client app has not been connected initially.");
        }

        // If the token is null, throw an error.
        if (_graphClientApp.AuthenticationResult is null)
        {
            throw new Exception("AuthenticationResult is null. The GraphClientApp was potentially not connected.");
        }

        // Initialize the return variable.
        string? apiResponse = null;

        // Check the authentication status and, if needed, refresh the token used for authentication.
        CheckAuthStatus();

        // Initialize the HttpRequestMessage with
        // the HTTP method and the endpoint of the API to call.
        HttpRequestMessage requestMessage = new(httpMethod, endpoint);

        // Add the authentication token to request message's Authorization header.
        requestMessage.Headers.Authorization = new("Bearer", _graphClientApp.AuthenticationResult.AccessToken);

        // Set the content of the request message, if a value was provided for apiPostBody.
        if (apiPostBody is not null && string.IsNullOrEmpty(apiPostBody) != true)
        {
            requestMessage.Content = new StringContent(apiPostBody);
            requestMessage.Content.Headers.ContentType = new("application/json");
        }

        // Start the process for sending the request message.
        // This will continue until 'isFinished' is set to true.
        bool isFinished = false;
        while (!isFinished)
        {
            // Send the request message and receive the response.
            HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);

            // Evaluate the response's HTTP status code.
            switch (responseMessage.StatusCode)
            {
                // If the status code is 429 (Too many requests)
                case HttpStatusCode.TooManyRequests:
                    // Get the 'RetryAfter' value from the response message.
                    RetryConditionHeaderValue? retryAfterValue = responseMessage.Headers.RetryAfter;

                    // Initialize the retryAfterBuffer time and set it.
                    // Adding a buffer can prevent the API from rejecting the next message.
                    TimeSpan retryAfterBuffer;
                    if (retryAfterValue is null || retryAfterValue.Delta is null)
                    {
                        // If the retryAfterValue is null, which is unlikely to happen,
                        // set a default value of 30 seconds for the retryAfterBuffer.
                        retryAfterBuffer = new(
                            hours: 0,
                            minutes: 0,
                            seconds: 30
                        );
                    }
                    else
                    {
                        // Otherwise, add 15 seconds to the timespan the API returned initially.
                        retryAfterBuffer = retryAfterValue.Delta.Value.Add(TimeSpan.FromSeconds(15));
                    }

                    // Wait for the set amount of seconds configured in 'retryAfterBuffer' and then resend the API call.
                    Console.WriteLine($"--- !!! Throttling for: {retryAfterBuffer.TotalSeconds} seconds !!! ---");
                    Thread.Sleep(retryAfterBuffer);
                    break;

                // Any other response is considered a "success".
                // Note: should expand this out to handle errors.
                default:
                    // Set the return value to what the API responded with.
                    apiResponse = await responseMessage.Content.ReadAsStringAsync();
                    responseMessage.Dispose();

                    // Stop the loop.
                    isFinished = true;
                    break;
            }
        }

        // Dispose the request message.
        requestMessage.Dispose();

        // Return the API's response.
        return apiResponse;
    }
}