using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedClassLibrary.DTOs.Events;
using System.Net.Http.Json;

namespace Back_office.Filters;

/// <summary>
/// Action filter that resolves the event title from the API when an <c>eventId</c> route parameter is present,
/// and stores it in <c>ViewData["EventTitle"]</c> for use in the shared layout.
/// </summary>
public class EventContextFilter(IHttpClientFactory httpClientFactory) : IAsyncActionFilter
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("DatabaseApi");

    /// <summary>
    /// Intercepts action execution to fetch and store the event title when an event context is present.
    /// </summary>
    /// <param name="context">The context for the action being executed.</param>
    /// <param name="next">The delegate to invoke the next action filter or action.</param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("eventId", out object? eventIdObj) && eventIdObj is int eventId && eventId > 0)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"/event/{eventId}");
                if (response.IsSuccessStatusCode)
                {
                    ApiResponse<EventDTO>? json = await response.Content.ReadFromJsonAsync<ApiResponse<EventDTO>>();
                    if (json?.Data != null && context.Controller is Controller controller)
                    {
                        controller.ViewData["EventTitle"] = json.Data.Title;
                    }
                }
            }
            catch
            {
                // Non-critical — navbar title is optional
            }
        }

        await next();
    }
}
