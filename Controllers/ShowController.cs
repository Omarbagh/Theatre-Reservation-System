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

    // Endpoint: GET http://localhost:5097/api/v1/shows
    [HttpGet] // Endpoint om alle shows op te halen
    public async Task<IActionResult> GetAllShows()
    {
        try
        {
            var shows = await _showService.GetShows(); // Haalt alle shows op via de showservice.
            return Ok(shows); // Geeft de shows terug als een HTTP 200 OK response.
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message); // Geeft een HTTP 500 statuscode terug bij een fout.
        }
    }

    // Endpoint: GET http://localhost:5097/api/v1/shows/{id}
    [HttpGet("{id}")] // HTTP GET om een specifieke show op te halen op basis van ID.
    public async Task<IActionResult> FindId(int id)
    {
        var show = await _showService.ShowWithId(id); // Haalt de show op met de opgegeven ID via de showservice.

        if (show == null)
        {
            return NotFound($"Show with ID {id} not found."); // Geeft een 404 Not Found terug als de show niet bestaat.
        }

        var jsonOptions = new JsonSerializerOptions // Configuratie voor JSON-serialisatie.
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles  // Negeert cyclische referenties.
        };

        return new JsonResult(show, jsonOptions); // Geeft de show terug als JSON.
    }


    // Endpoint: GET http://localhost:5097/api/v1/shows/filter/title?filter={filter}
    [HttpGet("filter/title")]  // HTTP GET om shows te filteren op titel.
    public async Task<IActionResult> FilteronTitle(string filter)
    {
        var shows = await _showService.ShowFilterTitle(filter); // Haalt shows op die aan de titelcriteria voldoen.
        return Ok(shows); // Geeft de gefilterde shows terug als een HTTP 200 OK response.
    }



    // Endpoint: GET http://localhost:5097/api/v1/shows/filter/location?location={location}
    [HttpGet("filter/location")] // HTTP GET om shows te filteren op locatie.
    public async Task<IActionResult> FilteronLocation([FromQuery] int location)
    {
        if (location <= 0)
        {
            return BadRequest("Invalid location ID."); // Geeft een 400 Bad Request terug als het locatie-ID ongeldig is.
        }

        var shows = await _showService.ShowFilterLocationAsync(location); // Haalt shows op voor de opgegeven locatie.

        if (shows == null || shows.Count == 0)
        {
            return NotFound($"No shows found for location ID {location}."); // Geeft een 404 Not Found terug als er geen shows zijn.
        }

        var jsonOptions = new JsonSerializerOptions // Configuratie voor JSON-serialisatie.
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles // Negeert cyclische referenties.
        };

        return new JsonResult(shows, jsonOptions); // Geeft de gefilterde shows terug als JSON.
    }

    // Endpoint: GET http://localhost:5097/api/v1/shows/filter/date?date1=2024-10-01&date2=2024-10-31&sortBy=date&sortOrder=asc
    // Endpoint: GET http://localhost:5097/api/v1/shows/filter/date?date1={date1}&date2={date2}&sortBy={sortBy}&sortOrder={sortOrder}
    [HttpGet("filter/date")] // HTTP GET om shows te filteren op datumbereik.
    public async Task<IActionResult> FilterByDateRange(
    [FromQuery] string date1,
    [FromQuery] string date2,
    [FromQuery] string sortBy = "date", // Standaard sorteren op datum.
    [FromQuery] string sortOrder = "asc" // Standaard sorteervolgorde is oplopend.
)
    {
        // Probeer de datums te parseren naar DateTime.
        if (!DateTime.TryParse(date1, out DateTime startDate) ||
            !DateTime.TryParse(date2, out DateTime endDate))
        {
            return BadRequest("Invalid date format. Please use a valid date format (e.g., yyyy-MM-dd)."); // Geeft een 400 Bad Request terug als het datumformaat ongeldig is.
        }

        // Zorg ervoor dat de einddatum groter is dan of gelijk aan de startdatum.
        if (endDate < startDate)
        {
            return BadRequest("The end date must be greater than or equal to the start date."); // Geeft een 400 Bad Request terug als de einddatum ongeldig is.
        }

        // Roep de service aan om de gefilterde shows te krijgen.
        var filteredShows = await _showService.FilterShowsByDateRangeAsync(startDate, endDate);

        if (!filteredShows.Any())
        {
            return NotFound("No shows found within the specified date range."); // Geeft een 404 Not Found terug als er geen shows zijn.
        }

        // Sort the filtered shows based on the sortBy and sortOrder parameters
        var sortedShows = SortShows(filteredShows, sortBy, sortOrder);

        var jsonOptions = new JsonSerializerOptions // Configuratie voor JSON-serialisatie.
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles // Negeert cyclische referenties.
        };

        return new JsonResult(sortedShows.ToList(), jsonOptions); // Geeft de gesorteerde shows terug als JSON.
    }

    private static IOrderedEnumerable<dynamic> SortShows(IEnumerable<dynamic> shows, string sortBy, string sortOrder)
    {
        IOrderedEnumerable<dynamic> sortedShows; // Variabele voor de gesorteerde shows.

        switch (sortBy.ToLower()) // Controleert op het veld waarop gesorteerd moet worden.
        {
            case "title":
                sortedShows = sortOrder.ToLower() == "desc"
                    ? shows.OrderByDescending(show => show.TheatreShowTitle) // Sorteert op titel in aflopende volgorde.
                    : shows.OrderBy(show => show.TheatreShowTitle); // Sorteert op titel in oplopende volgorde.
                break;

            case "price":
                sortedShows = sortOrder.ToLower() == "desc"
                    ? shows.OrderByDescending(show => show.Price) // Sorteert op prijs in aflopende volgorde.
                    : shows.OrderBy(show => show.Price); // Sorteert op prijs in oplopende volgorde.
                break;

            case "date":
            default:
                sortedShows = sortOrder.ToLower() == "desc"
                    ? shows.OrderByDescending(show => show.DateAndTime) // Sorteert op datum in aflopende volgorde.
                    : shows.OrderBy(show => show.DateAndTime); // Sorteert op datum in oplopende volgorde.
                break;
        }

        return sortedShows; // Geeft de gesorteerde shows terug.
    }

    // Endpoint: POST http://localhost:5097/api/v1/shows/AddShow
    [HttpPost("AddShow")] // HTTP POST om een nieuwe show toe te voegen.
    [ServiceFilter(typeof(AdminAuthFilter))]

    public async Task<IActionResult> CreateShow([FromBody] TheatreShow newShow)
    {

        var result = await _showService.CreateShowAsync(newShow); // Roept de service aan om de nieuwe show aan te maken.

        if (result == "Venue not found.")
        {
            return NotFound(result); // Geeft een 404 Not Found terug als de locatie niet bestaat.
        }

        return Ok(result); // Geeft de resultaat terug als een HTTP 200 OK response.
    }

    // Endpoint: PUT http://localhost:5097/api/v1/shows/UpdateShow/{id}
    [HttpPut("UpdateShow/{id}")] // HTTP PUT om een bestaande show bij te werken op basis van ID.
    [ServiceFilter(typeof(AdminAuthFilter))]
    public async Task<IActionResult> UpdateShow(int id, [FromBody] TheatreShow show)
    {
        var result = await _showService.UpdateShowAsync(id, show); // Roept de service aan om de show bij te werken.

        if (result == "Show not found.")
        {
            return NotFound(result); // Geeft een 404 Not Found terug als de show niet bestaat.
        }

        return Ok(result); // Geeft de resultaat terug als een HTTP 200 OK response.
    }

    // Endpoint: DELETE http://localhost:5097/api/v1/shows/DeleteShow/{id}
    [HttpDelete("DeleteShow/{id}")] // HTTP DELETE om een show te verwijderen op basis van ID.
    [ServiceFilter(typeof(AdminAuthFilter))]
    public async Task<IActionResult> DeleteShow(int id)
    {

        var result = await _showService.DeleteShowAsync(id); // Roept de service aan om de show te verwijderen.

        if (result == "Show not found.")
        {
            return NotFound(result); // Geeft een 404 Not Found terug als de show niet bestaat.
        }

        return Ok(result); // Geeft de resultaat terug als een HTTP 200 OK response.
    }
}
