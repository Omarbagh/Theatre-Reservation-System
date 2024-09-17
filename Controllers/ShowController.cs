using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


[Route("api/v1/shows")]
[ApiController]
public class ShowController : ControllerBase
{
    private readonly string jsonFilePath = "Data/shows.json";

    [HttpGet]
    public async Task<IActionResult> GetAllShows()
    {
        var shows = await ReadShowsFromJsonAsync();

        // Debug log
        Console.WriteLine($"API Response: {JsonSerializer.Serialize(shows)}");

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
            Console.WriteLine("File does not exist.");
            return null;
        }

        var jsonData = await System.IO.File.ReadAllTextAsync(jsonFilePath);
        Console.WriteLine($"Read JSON data: {jsonData}");


        var shows = JsonSerializer.Deserialize<List<Show>>(jsonData);
        if (shows == null)
        {
            Console.WriteLine("Deserialization returned null.");
        }
        return shows;


    }
}


public class Show
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("genre")]
    public string Genre { get; set; }
}

