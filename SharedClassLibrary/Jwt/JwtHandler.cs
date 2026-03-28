using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Headers;

namespace SharedClassLibrary.Jwt;

/// <summary>
/// A delegating handler that intercepts outgoing HTTP requests to append a JWT access token to the Authorization header from cookies.
/// It also handles unauthorized (401) responses by attempting to refresh the token and retrying the request.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="JwtHandler"/> class.
/// </remarks>
/// <param name="httpContextAccessor">Provides access to the current <see cref="HttpContext"/>.</param>
/// <param name="httpClientFactory">A factory component for creating <see cref="HttpClient"/> instances.</param>
public class JwtHandler(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory) : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    /// <summary>
    /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
    /// Attaches the JWT from the current HTTP context and attempts to refresh the token if a 401 Unauthorized response is received.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The HTTP response message.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? token = _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized ||
            response.Headers.Contains("X-Retry") ||
            request.RequestUri?.AbsolutePath.Contains("api/auth/refresh") == true)
        {
            return response;
        }

        HttpClient httpClient = _httpClientFactory.CreateClient("ApiClient");
        HttpResponseMessage refreshResponse = await httpClient.PostAsync("http://databaseapi:5000/api/auth/refresh", null, cancellationToken);

        if (!refreshResponse.IsSuccessStatusCode)
        {
            return response;
        }

        request.Headers.Add("X-Retry", "true");

        HttpRequestMessage newRequest = await CloneHttpRequestMessage(request);
        newRequest.Headers.Add("X-Retry", "true");
        string? newToken = _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];

        if (!string.IsNullOrEmpty(newToken))
        {
            newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
        }

        return await base.SendAsync(newRequest, cancellationToken);
    }

    /// <summary>
    /// Clones an <see cref="HttpRequestMessage"/> so that it can be resent after a token refresh.
    /// </summary>
    /// <param name="request">The original HTTP request message to clone.</param>
    /// <returns>A new <see cref="HttpRequestMessage"/> that is a copy of the original request.</returns>
    private static async Task<HttpRequestMessage> CloneHttpRequestMessage(HttpRequestMessage request)
    {
        HttpRequestMessage clone = new(request.Method, request.RequestUri);

        foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (request.Content != null)
        {
            MemoryStream memoryStream = new();
            await request.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            clone.Content = new StreamContent(memoryStream);

            foreach (KeyValuePair<string, IEnumerable<string>> header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clone;
    }
}
