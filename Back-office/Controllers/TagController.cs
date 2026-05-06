using SharedClassLibrary.DTOs.Tags;
using Microsoft.AspNetCore.Mvc;

namespace Back_office.Controllers;

[Route("{eventId}/[controller]")]
/// <summary>
/// Controller responsible for handling tag management (CRUD) for a specific event.
/// </summary>
public class TagController : Controller
{
    private readonly HttpClient client;

    /// <summary>
    /// Initializes the controller with an HttpClient for communicating with the Database API.
    /// </summary>
    public TagController(IHttpClientFactory httpClientFactory)
    {
        client = httpClientFactory.CreateClient("DatabaseApi");
    }

    /// <summary>
    /// Displays the tag management page for a specific event.
    /// </summary>
    public IActionResult Index(int eventId)
    {
        ViewData["EventId"] = eventId;
        return View(eventId);
    }

    /// <summary>
    /// Gets all tags for a specific event. Used by JavaScript on tag/index.cshtml.
    /// </summary>
    [HttpGet("data")]
    public async Task<IActionResult> GetTagsForEvent(int eventId)
    {
        HttpResponseMessage response = await client.GetAsync($"/tag?eventId={eventId}");

        ApiResponse<List<TagResponseDTO>>? apiResponse =
            await response.Content.ReadFromJsonAsync<ApiResponse<List<TagResponseDTO>>>();

        return StatusCode((int)response.StatusCode, apiResponse);
    }

    /// <summary>
    /// Creates or updates a tag depending on whether IdTag is set. Used by JavaScript on tag/index.cshtml.
    /// </summary>
    [HttpPost("data")]
    public async Task<IActionResult> SaveTag([FromBody] TagResponseDTO tag)
    {
        HttpResponseMessage response;

        // CREATE
        if (tag.IdTag == 0)
        {
            response = await client.PostAsJsonAsync("/tag", tag);
        }
        // UPDATE
        else
        {
            response = await client.PutAsJsonAsync($"/tag/{tag.IdTag}", tag);
        }

        string content = await response.Content.ReadAsStringAsync();
        return StatusCode((int)response.StatusCode, content);
    }

    /// <summary>
    /// Deletes a tag for a specific event using its ID.
    /// </summary>
    [HttpDelete("{idTag}")]
    public async Task<IActionResult> DeleteTag(int idTag)
    {
        HttpResponseMessage response = await client.DeleteAsync($"/tag/{idTag}");

        string content = await response.Content.ReadAsStringAsync();
        return StatusCode((int)response.StatusCode, content);
    }
}
