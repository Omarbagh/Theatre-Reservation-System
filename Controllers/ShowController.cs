using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using StarterKit.Models;
using Microsoft.EntityFrameworkCore;

using System.Text.Json;


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


        // 1.3 Add a parameter that allows you to filter the list of shows by title or description.


        if (shows.Count == 0)
        {
            return NotFound("No shows available.");
        }

        return Ok(shows);
    }
    [HttpGet("id/{id}")]
    public async Task<IActionResult> FindId(int id)
    {
        var show = await _context.TheatreShow
                                 .Include(s => s.Venue)
                                 .Include(s => s.theatreShowDates)
                                 .FirstOrDefaultAsync(s => s.TheatreShowId == id);

        if (show == null)
        {
            return NotFound($"Show with ID {id} not found.");
        }

        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
        };

        return new JsonResult(show, jsonOptions);
    }



    [HttpGet("filter/title")]
    public async Task<IActionResult> FilteronTitle(string filter)
    {
        var shows = await _context.TheatreShow.ToListAsync();

        if (!string.IsNullOrEmpty(filter))
        {
            shows = shows
                .Where(ts =>
                    (ts.Title != null && ts.Title.Contains(filter, StringComparison.OrdinalIgnoreCase)) ||
                    (ts.Description != null && ts.Description.Contains(filter, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        return Ok(shows);
    }


    [HttpGet("filter/location")]
    public async Task<IActionResult> FilteronLocation([FromQuery] int location)
    {
        var shows = await _context.Venue.Include(x => x.TheatreShows).ToListAsync();

        if (location > 0)
        {
            shows = shows.Where(ts => ts.VenueId != null && ts.VenueId == location).ToList();
        }

        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };

        return new JsonResult(shows, jsonOptions);
    }


    [HttpGet("filter/date")]
    public async Task<IActionResult> FilterByDateRange([FromQuery] string date1, [FromQuery] string date2)
    {
        if (!DateTime.TryParse(date1, out DateTime startDate) ||
            !DateTime.TryParse(date2, out DateTime endDate))
        {
            return BadRequest("Invalid date format. Please use a valid date format (e.g., yyyy-MM-dd).");
        }

        if (endDate < startDate)
        {
            return BadRequest("The end date must be greater than or equal to the start date.");
        }

        var venues = await _context.Venue
            .Include(v => v.TheatreShows)
                .ThenInclude(ts => ts.theatreShowDates)
            .ToListAsync();

        var filteredShows = venues.SelectMany(v => v.TheatreShows, (v, ts) => new { Venue = v, TheatreShow = ts })
            .SelectMany(ts => ts.TheatreShow.theatreShowDates
                .Where(tsd => tsd.DateAndTime >= startDate && tsd.DateAndTime <= endDate)
                .Select(tsd => new
                {
                    tsd.TheatreShowDateId,
                    tsd.DateAndTime,
                    TheatreShowTitle = ts.TheatreShow.Title,
                    VenueName = ts.Venue.Name
                }))
            .ToList();

        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };

        return new JsonResult(filteredShows, jsonOptions);

    }






    [HttpPost("AddShow")]
    public async Task<IActionResult> CreateShow([FromBody] TheatreShow newShow)
    {
        var adminCheckResult = IsAdminLoggedIn();

        if (!adminCheckResult)
        {
            return Unauthorized("Only admins can create new shows.");
        }

        try
        {
            // Detach the venue if it exists to avoid re-inserting it
            var existingVenue = await _context.Venue.FindAsync(newShow.Venue.VenueId);

            if (existingVenue == null)
            {
                return NotFound("Venue not found.");
            }

            // Assign the existing venue to the new show
            newShow.Venue = existingVenue;

            _context.TheatreShow.Add(newShow);
            await _context.SaveChangesAsync();
            return Ok("Show created successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while creating the show: " + ex.Message);
        }
    }
    [HttpPut("UpdateShow/{id}")]
    public async Task<IActionResult> UpdateShow(int id, [FromBody] TheatreShow show)
    {
        var adminCheckResult = IsAdminLoggedIn();

        if (!adminCheckResult)
        {
            return Unauthorized("Only admins can update shows.");
        }

        // Find the existing show by id
        var oldShow = await _context.TheatreShow.Include(s => s.theatreShowDates)
                                                .Include(s => s.Venue)
                                                .FirstOrDefaultAsync(s => s.TheatreShowId == id);

        if (oldShow == null)
        {
            return NotFound("Show not found.");
        }

        try
        {
            // Only update fields if they have valid values in the incoming request

            if (!string.IsNullOrEmpty(show.Title))
            {
                oldShow.Title = show.Title;
            }

            if (!string.IsNullOrEmpty(show.Description))
            {
                oldShow.Description = show.Description;
            }

            if (show.Price > 0)
            {
                oldShow.Price = show.Price;
            }

            // Update venue if provided and valid
            if (show.Venue != null && oldShow.Venue.VenueId != show.Venue.VenueId)
            {
                var newVenue = await _context.Venue.FindAsync(show.Venue.VenueId);
                if (newVenue == null)
                {
                    return NotFound("Venue not found.");
                }
                oldShow.Venue = newVenue;
            }

            // Update TheatreShowDates if provided
            if (show.theatreShowDates != null && show.theatreShowDates.Any())
            {
                _context.TheatreShowDate.RemoveRange(oldShow.theatreShowDates);

                foreach (var date in show.theatreShowDates)
                {
                    oldShow.theatreShowDates.Add(new TheatreShowDate
                    {
                        DateAndTime = date.DateAndTime
                    });
                }
            }

            // Save changes
            _context.TheatreShow.Update(oldShow);
            await _context.SaveChangesAsync();

            return Ok("Show updated successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the show: {ex.Message}");
        }
    }



    [HttpDelete("DeleteShow/{id}")]
    public async Task<IActionResult> DeleteShow(int id)
    {
        var adminCheckResult = IsAdminLoggedIn();

        if (!adminCheckResult)
        {
            return Unauthorized("Only admins can create new shows.");
        }

        // Find the show by id
        var show = await _context.TheatreShow.Include(s => s.theatreShowDates).FirstOrDefaultAsync(s => s.TheatreShowId == id);

        if (show == null)
        {
            return NotFound("Show not found.");
        }
        //lala
        try
        {
            // Remove the associated TheatreShowDates first
            _context.TheatreShowDate.RemoveRange(show.theatreShowDates);

            // Then remove the show
            _context.TheatreShow.Remove(show);

            // Save changes to the database
            await _context.SaveChangesAsync();
            return Ok("Show deleted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the show: {ex.Message}");
        }
    }

    private bool IsAdminLoggedIn()
    {
        var username = _httpContextAccessor.HttpContext?.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            return false;
        }

        var admin = _context.Admin
            .Where(a => a.UserName == username)
            .Select(a => a.UserName)
            .FirstOrDefault();

        return admin != null;
    }


}
