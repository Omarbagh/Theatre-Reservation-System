using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Threading.Tasks;

namespace StarterKit.Controllers
{
    [Route("api/v1/admin/reservations")]
    [ApiController]
    [ServiceFilter(typeof(AdminAuthFilter))] // Voert een filter uit om te garanderen dat alleen admins toegang hebben tot deze endpoints.
    public class ReservationManagementController : ControllerBase
    {
        private readonly ReservationManagementService _reservationService;

        public ReservationManagementController(ReservationManagementService reservationService)
        {
            _reservationService = reservationService;
        }

        // Endpoint: GET http://localhost:5097/api/v1/admin/reservations
        // Endpoint: GET http://localhost:5097/api/v1/admin/reservations?showId={showId}
        // Endpoint: GET http://localhost:5097/api/v1/admin/reservations?date={yyyy-MM-dd}
        // Endpoint: GET http://localhost:5097/api/v1/admin/reservations?showId={showId}&date={yyyy-MM-dd}   
        [HttpGet] // HTTP GET om alle reserveringen op te halen.
        public async Task<IActionResult> GetReservations([FromQuery] int? showId, [FromQuery] DateTime? date)
        {
            var reservations = await _reservationService.GetReservations(showId, date); // Haalt reserveringen op op basis van showId en datum.
            return Ok(reservations); // Geeft de reserveringen terug als een HTTP 200 OK response.
        }

        // Endpoint: GET http://localhost:5097/api/v1/admin/reservations/search?email={email}
        // Endpoint: GET http://localhost:5097/api/v1/admin/reservations/search?email={email}&reservationId={reservationId}
        [HttpGet("search")] // HTTP GET om reserveringen te zoeken op e-mail of reserverings-ID.
        public async Task<IActionResult> SearchReservations([FromQuery] string email, [FromQuery] int? reservationId)
        {
            var reservations = await _reservationService.SearchReservations(email, reservationId); // Zoekt reserveringen op basis van e-mail of reserverings-ID.
            return Ok(reservations); // Geeft de zoekresultaten terug als een HTTP 200 OK response.
        }

        // Endpoint: PUT http://localhost:5097/api/v1/admin/reservations/{id}/mark-used
        [HttpPut("{id}/mark-used")] // HTTP PUT om een reservering als gebruikt te markeren.
        public async Task<IActionResult> MarkReservationAsUsed(int id)
        {
            var result = await _reservationService.MarkReservationAsUsed(id); // Markeert een reservering als gebruikt op basis van de reserverings-ID.
            if (result.Contains("not found"))
            {
                return NotFound(result); // Geeft een 404 Not Found terug als de reserverings-ID ongeldig is.
            }
            return Ok(result); // Geeft een succesmelding terug als een HTTP 200 OK response.
        }

        // Endpoint: DELETE http://localhost:5097/api/v1/admin/reservations/{id}
        [HttpDelete("{id}")] // HTTP DELETE om een reservering te verwijderen.
        public async Task<IActionResult> DeleteReservation(int id) // Verwijdert een reservering op basis van de reserverings-ID.
        {
            var result = await _reservationService.DeleteReservation(id);
            if (result.Contains("not found"))
            {
                return NotFound(result); // Geeft een 404 Not Found terug als de reservering niet bestaat.
            }
            return Ok(result); // Geeft een succesmelding terug als een HTTP 200 OK response.
        }
    }
}
