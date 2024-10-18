using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using Services;
using System.Threading.Tasks;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/v1/snacks/Reservation")]
    public class SnackReservationController : ControllerBase
    {
        private readonly SnackReservationService _snackService;

        public SnackReservationController(SnackReservationService snackService)
        {
            _snackService = snackService;
        }
        [HttpGet]
        public async Task<IActionResult> Getsnacks()
        {
            var snacks = await _snackService.GetAllSnacksAsync();
            if (snacks.Count == 0)
            {
                return NoContent();
            }
            return Ok(snacks);
        }

        [HttpPost]
        public async Task<IActionResult> OrderSnacks([FromBody] SnackReservationRequest snackReservationRequest)
        {
            if (snackReservationRequest == null || snackReservationRequest.orderlistt == null || snackReservationRequest.orderlistt.Count == 0)
            {
                return BadRequest("No snack orders were provided.");
            }

            var result = await _snackService.PlaceSnackOrder(snackReservationRequest.reservationId, snackReservationRequest.orderlistt);
            if (result == null)
            {
                return BadRequest("reservation id doesn't exist");
            }
            return Ok(result);
        }

    }
}
public class SnackReservationRequest
{
    public int? reservationId { get; set; }
    public List<SnackOrder> orderlistt { get; set; }
}

public class SnackOrder
{
    public int? id { get; set; }
    public int? amount { get; set; }
}