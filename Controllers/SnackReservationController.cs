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
        public async Task<IActionResult> OrderSnacks([FromBody] List<SnackOrder> snackOrders)
        {
            if (snackOrders == null || snackOrders.Count == 0)
            {
                return BadRequest("No snack orders were provided.");
            }

            var result = await _snackService.PlaceSnackOrder(snackOrders);
            return Ok(result);
        }

    }
}
public class SnackOrder
{
    public int? id { get; set; }
    public int? amount { get; set; }
}