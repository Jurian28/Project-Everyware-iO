using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Headers;

namespace SharedClassLibrary.Jwt;

public class JwtHandler(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory) : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? token = _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized || response.Headers.Contains("X-Retry"))
        {
            return response;
        }

        HttpClient httpClient = _httpClientFactory.CreateClient("ApiClient");
        string apiBaseUrl = GetApiBaseUrl();
        HttpResponseMessage refreshResponse = await httpClient.PostAsync($"{apiBaseUrl}/api/auth/refresh", null, cancellationToken);

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

    private static string GetApiBaseUrl()
    {
        if (Environment.GetEnvironmentVariable("RUNNING_IN_DOCKER") == "true")
        {
            return "http://databaseapi:5000";
        }
        else
        {
            string apiBaseUrl = Environment.GetEnvironmentVariable("APP_URL") ?? throw new InvalidOperationException("APP_URL environment variable not set.");
            string databaseApiPort = Environment.GetEnvironmentVariable("DATABASE_API_PORT") ?? throw new InvalidOperationException("DATABASE_API_PORT environment variable not set.");
            
            return $"{apiBaseUrl}:{databaseApiPort}";
        }
    }
}
