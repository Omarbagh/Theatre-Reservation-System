using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using StarterKit.Models;

[Route("api/v1/shows")]
[ApiController]
public class ShowController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllShows()
    {
        string connectionString = @"Data Source=webdev.sqlite";
        string query = "SELECT * FROM TheatreShow";

        List<TheatreShow> shows = new List<TheatreShow>();

        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqliteCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var show = new TheatreShow
                        {
                            TheatreShowId = reader.GetInt32(reader.GetOrdinal("TheatreShowId")),
                            Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                            Price = reader.GetDouble(reader.GetOrdinal("Price")),

                        };
                        shows.Add(show);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return StatusCode(500, "Internal server error.");
        }

        if (shows.Count == 0)
        {
            return NotFound("No shows available.");
        }

        return Ok(shows);
    }
}
