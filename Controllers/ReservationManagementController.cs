using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Threading.Tasks;

namespace StarterKit.Controllers
{
    [Route("api/v1/admin/reservations")]
    [ApiController]
    [ServiceFilter(typeof(AdminAuthFilter))]
    public class ReservationManagementController : ControllerBase
    {
        private readonly ReservationManagementService _reservationService;

        public ReservationManagementController(ReservationManagementService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReservations([FromQuery] int? showId, [FromQuery] DateTime? date)
        {
            var reservations = await _reservationService.GetReservations(showId, date);
            return Ok(reservations);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchReservations([FromQuery] string email, [FromQuery] int? reservationId)
        {
            var reservations = await _reservationService.SearchReservations(email, reservationId);
            return Ok(reservations);
        }

        [HttpPut("{id}/mark-used")]
        public async Task<IActionResult> MarkReservationAsUsed(int id)
        {
            var result = await _reservationService.MarkReservationAsUsed(id);
            if (result.Contains("not found"))
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var result = await _reservationService.DeleteReservation(id);
            if (result.Contains("not found"))
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
