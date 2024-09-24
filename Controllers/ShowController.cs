using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using StarterKit.Models;
using Microsoft.EntityFrameworkCore;


[Route("api/v1/shows")]
[ApiController]
public class ShowController : ControllerBase
{

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DatabaseContext _context;

    public ShowController(IHttpContextAccessor httpContextAccessor, DatabaseContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllShows()
    {
        string connectionString = @"Data Source=webdev.sqlite";
        string query = @"
            SELECT 
                ts.TheatreShowId, 
                ts.Title, 
                ts.Description, 
                ts.Price, 
                v.VenueId, 
                v.Name as VenueName, 
                v.Capacity as VenueCapacity,
                tsd.TheatreShowDateId, 
                tsd.DateAndTime as ShowDate
            FROM 
                TheatreShow ts
            LEFT JOIN 
                Venue v ON ts.VenueId = v.VenueId
            LEFT JOIN 
                TheatreShowDate tsd ON ts.TheatreShowId = tsd.TheatreShowId
            ORDER BY 
                ts.TheatreShowId, tsd.DateAndTime;
        ";

        List<TheatreShow> shows = new List<TheatreShow>();

        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqliteCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    // Dictionary to avoid duplicate shows
                    var showMap = new Dictionary<int, TheatreShow>();

                    while (await reader.ReadAsync())
                    {
                        int showId = reader.GetInt32(reader.GetOrdinal("TheatreShowId"));

                        // Checken of de show al in de map zit zo niet maakt die hem aan.
                        if (!showMap.ContainsKey(showId))
                        {
                            var show = new TheatreShow
                            {
                                TheatreShowId = showId,
                                Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                Price = reader.GetDouble(reader.GetOrdinal("Price")),
                                Venue = new Venue
                                {
                                    VenueId = reader.GetInt32(reader.GetOrdinal("VenueId")),
                                    Name = reader.IsDBNull(reader.GetOrdinal("VenueName")) ? null : reader.GetString(reader.GetOrdinal("VenueName")),
                                    Capacity = reader.GetInt32(reader.GetOrdinal("VenueCapacity"))
                                },
                                theatreShowDates = new List<TheatreShowDate>()
                            };

                            showMap[showId] = show;
                        }

                        // Add the show date if available
                        if (!reader.IsDBNull(reader.GetOrdinal("ShowDate")))
                        {
                            var showDate = new TheatreShowDate
                            {
                                TheatreShowDateId = reader.GetInt32(reader.GetOrdinal("TheatreShowDateId")),
                                DateAndTime = reader.GetDateTime(reader.GetOrdinal("ShowDate"))
                            };

                            showMap[showId].theatreShowDates.Add(showDate);
                        }
                    }

                    // Convert the dictionary to a list
                    shows = new List<TheatreShow>(showMap.Values);
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

    [HttpPost("AddShow")]
    public async Task<IActionResult> CreateShow([FromBody] TheatreShow newShow)
    {
        var adminCheckResult = await IsAdminLoggedIn();

        if (!adminCheckResult)
        {
            return Unauthorized("Only admins can create new shows.");
        }

        try
        {
            _context.TheatreShow.Add(newShow);
            await _context.SaveChangesAsync();
            return Ok("Show created successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while creating the show: " + ex.Message);
        }
    }

    private async Task<bool> IsAdminLoggedIn()
    {
        var username = _httpContextAccessor.HttpContext?.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            return false;
        }

        var admin = await _context.Admin
            .Where(a => a.UserName == username)
            .Select(a => a.UserName)
            .FirstOrDefaultAsync();

        return admin != null;
    }
    
}
