using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using StarterKit.Models;
using Microsoft.EntityFrameworkCore;

using System.Text.Json;
using Services;


[Route("api/v1/shows")]
[ApiController]
public class ShowController : ControllerBase
{

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DatabaseContext _context;
    private readonly ShowService _showService;

    public ShowController(IHttpContextAccessor httpContextAccessor, DatabaseContext context, ShowService showService)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _showService = showService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllShows()
    {
        try
        {
            var shows = await _showService.GetShows();
            return Ok(shows);
        }
        catch (Exception ex)
        {
            // Log the exception (e.g., using a logging framework)
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> FindId(int id)
    {
        // Call the service method to get the show by ID
        var show = await _showService.ShowWithId(id); // Assuming you have injected ShowService

        if (show == null)
        {
            return NotFound($"Show with ID {id} not found.");
        }

        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };

        return new JsonResult(show, jsonOptions);
    }



    [HttpGet("filter/title")]
    public async Task<IActionResult> FilteronTitle(string filter)
    {
        var shows = await _showService.ShowFilterTitle(filter);
        return Ok(shows);
    }




    [HttpGet("filter/location")]
    public async Task<IActionResult> FilteronLocation([FromQuery] int location)
    {
        if (location <= 0)
        {
            return BadRequest("Invalid location ID.");
        }

        var shows = await _showService.ShowFilterLocationAsync(location);

        if (shows == null || shows.Count == 0)
        {
            return NotFound($"No shows found for location ID {location}.");
        }

        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };

        return new JsonResult(shows, jsonOptions);
    }



    [HttpGet("filter/date")]
    public async Task<IActionResult> FilterByDateRange(
    [FromQuery] string date1,
    [FromQuery] string date2,
    [FromQuery] string sortBy = "date", // Default sort by date
    [FromQuery] string sortOrder = "asc" // Default sort order is ascending
)
    {
        // Parse the date strings to DateTime
        if (!DateTime.TryParse(date1, out DateTime startDate) ||
            !DateTime.TryParse(date2, out DateTime endDate))
        {
            return BadRequest("Invalid date format. Please use a valid date format (e.g., yyyy-MM-dd).");
        }

        // Ensure the end date is greater than or equal to the start date
        if (endDate < startDate)
        {
            return BadRequest("The end date must be greater than or equal to the start date.");
        }

        // Call the service to get the filtered shows
        var filteredShows = await _showService.FilterShowsByDateRangeAsync(startDate, endDate);

        if (!filteredShows.Any())
        {
            return NotFound("No shows found within the specified date range.");
        }

        // Sort the filtered shows based on the sortBy and sortOrder parameters
        var sortedShows = SortShows(filteredShows, sortBy, sortOrder);

        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };

        return new JsonResult(sortedShows.ToList(), jsonOptions);
    }

    private static IOrderedEnumerable<dynamic> SortShows(IEnumerable<dynamic> shows, string sortBy, string sortOrder)
    {
        IOrderedEnumerable<dynamic> sortedShows;

        switch (sortBy.ToLower())
        {
            case "title":
                sortedShows = sortOrder.ToLower() == "desc"
                    ? shows.OrderByDescending(show => show.TheatreShowTitle)
                    : shows.OrderBy(show => show.TheatreShowTitle);
                break;

            case "price":
                sortedShows = sortOrder.ToLower() == "desc"
                    ? shows.OrderByDescending(show => show.Price)
                    : shows.OrderBy(show => show.Price);
                break;

            case "date":
            default:
                sortedShows = sortOrder.ToLower() == "desc"
                    ? shows.OrderByDescending(show => show.DateAndTime)
                    : shows.OrderBy(show => show.DateAndTime);
                break;
        }

        return sortedShows;
    }


    [HttpPost("AddShow")]
    public async Task<IActionResult> CreateShow([FromBody] TheatreShow newShow)
    {
        if (!IsAdminLoggedIn())
        {
            return Unauthorized("Only admins can create new shows.");
        }

        var result = await _showService.CreateShowAsync(newShow);

        if (result == "Venue not found.")
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPut("UpdateShow/{id}")]
    public async Task<IActionResult> UpdateShow(int id, [FromBody] TheatreShow show)
    {
        if (!IsAdminLoggedIn())
        {
            return Unauthorized("Only admins can update shows.");
        }

        var result = await _showService.UpdateShowAsync(id, show);

        if (result == "Show not found.")
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpDelete("DeleteShow/{id}")]
    public async Task<IActionResult> DeleteShow(int id)
    {
        if (!IsAdminLoggedIn())
        {
            return Unauthorized("Only admins can delete shows.");
        }

        var result = await _showService.DeleteShowAsync(id);

        if (result == "Show not found.")
        {
            return NotFound(result);
        }

        return Ok(result);
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
