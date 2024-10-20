using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/v1/Reservation")]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost] // HTTP POST om een nieuwe reservering te creëren.
    public async Task<IActionResult> CreateReservation([FromBody] ReservationDto reservationDto)
    {
        // ModelState bevat informatie over de status van de gegevens die naar de server zijn verzonden.
        // Het controleert of de gegevens geldig zijn volgens de validatieregels van het model. 
        // Als het model ongeldig is, kan de controller een foutmelding teruggeven aan de client.
        // Controleer of het model geldig is; als dit niet het geval is, geef een Bad Request terug met modelstatus.
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Geeft een 400 Bad Request terug met de modelstatus.
        }

        try
        {
            // Roep de service aan om de reservering aan te maken.
            var result = await _reservationService.CreateReservationAsync(reservationDto);
            return CreatedAtAction(nameof(CreateReservation), new { id = result.ReservationId }, result); // Geeft een 201 Created terug met de locatie van de nieuwe reservering.
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message); // Geeft een 404 Not Found terug als een sleutel niet is gevonden.
        }
        catch
        {
            // Vang elke andere uitzondering op en geef een 500 Internal Server Error terug.
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
