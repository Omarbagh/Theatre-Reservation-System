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
        [ServiceFilter(typeof(AdminAuthFilter))] // Voert een filter uit om te garanderen dat alleen admins toegang hebben tot deze endpoints.
        [HttpGet] // HTTP GET om alle snacks op te halen.
        public async Task<IActionResult> Getsnacks()
        {
            var snacks = await _snackService.GetAllSnacksAsync(); // Haalt alle snacks op via de snackreservatieservice.
            if (snacks.Count == 0) // Controleert of er geen snacks zijn.
            {
                return NoContent(); // Geeft een HTTP 204 No Content terug als er geen snacks zijn.

            }
            return Ok(snacks); // Geeft de snacks terug als een HTTP 200 OK response.
        }

        [HttpPost] // HTTP POST om snackbestellingen te plaatsen.
        public async Task<IActionResult> OrderSnacks([FromBody] SnackReservationRequest snackReservationRequest)
        {
            // Controleert of de snackreservatieaanvraag geldig is.
            if (snackReservationRequest == null || snackReservationRequest.orderlistt == null || snackReservationRequest.orderlistt.Count == 0)
            {
                return BadRequest("No snack orders were provided."); // Geeft een 400 Bad Request terug als er geen bestellingen zijn.
            }

            var result = await _snackService.PlaceSnackOrder(snackReservationRequest.reservationId, snackReservationRequest.orderlistt); // Plaatst de snackbestelling via de service.
            if (result == null) // Controleert of de reserverings-ID niet bestaat.
            {
                return BadRequest("reservation id doesn't exist"); // Geeft een 400 Bad Request terug als de reserverings-ID ongeldig is.
            }
            return Ok(result); // Geeft het resultaat terug als een HTTP 200 OK response.
        }

    }
}

// Definieert de klasse voor de snackreservatieaanvraag.
public class SnackReservationRequest
{
    public int? reservationId { get; set; } // De ID van de reservering (optioneel).
    public List<SnackOrder> orderlistt { get; set; } // Lijst van snackbestellingen.
}

// Definieert de klasse voor een snackbestelling.
public class SnackOrder
{
    public int? id { get; set; } // De ID van de snack (optioneel).
    public int? amount { get; set; } // Het aantal van de snack dat besteld wordt (optioneel).
}