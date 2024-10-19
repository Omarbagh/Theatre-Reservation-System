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

    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationDto reservationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _reservationService.CreateReservationAsync(reservationDto);
            return CreatedAtAction(nameof(CreateReservation), new { id = result.ReservationId }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
