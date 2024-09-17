using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ShowController : ControllerBase
{
    private readonly string jsonFilePath = "Data/shows.json";

    // GET: api/show
    [HttpGet]
    public async Task<IActionResult> GetAllShows()
    {
        var shows = await ReadShowsFromJsonAsync();

        if (shows == null || shows.Count == 0)
        {
            return NotFound("No shows available.");
        }

        return Ok(shows);
    }
    private async Task<List<Show>> ReadShowsFromJsonAsync()
    {
        if (!System.IO.File.Exists(jsonFilePath))
        {
            return null;
        }

        var jsonData = await System.IO.File.ReadAllTextAsync(jsonFilePath);
        return JsonSerializer.Deserialize<List<Show>>(jsonData);
    }
}

public class Show
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Genre { get; set; }
}
