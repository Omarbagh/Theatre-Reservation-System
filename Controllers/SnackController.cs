using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using Services;
using System.Threading.Tasks;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/v1/snacks")]
    public class SnackController : ControllerBase
    {
        private readonly SnackService _snackService;

        public SnackController(SnackService snackService)
        {
            _snackService = snackService;
        }

        // GET: api/v1/snacks
        [HttpGet] // HTTP GET om alle snacks op te halen.
        public async Task<IActionResult> GetSnacks()
        {
            var snacks = await _snackService.GetAllSnacksAsync(); // Haalt alle snacks op via de snackservice.
            if (snacks.Count == 0) // Controleert of er geen snacks zijn.
            {
                return NoContent(); // Geeft een HTTP 204 No Content terug als er geen snacks zijn.
            }
            return Ok(snacks); // Geeft de snacks terug als een HTTP 200 OK response.
        }

        // GET: api/v1/snacks/{id}
        [HttpGet("{id}")] // HTTP GET om een specifieke snack op te halen op basis van ID.
        public async Task<IActionResult> GetSnackById(int id)
        {
            var snack = await _snackService.GetSnackByIdAsync(id); // Haalt de snack op met de opgegeven ID via de snackservice.
            if (snack == null) // Controleert of de snack niet bestaat.
            {
                return NotFound(); // Geeft een 404 Not Found terug als de snack niet bestaat.
            }
            return Ok(snack); // Geeft de snack terug als een HTTP 200 OK response.
        }

        // POST: api/v1/snacks
        [HttpPost] // HTTP POST om een nieuwe snack toe te voegen.
        [ServiceFilter(typeof(AdminAuthFilter))] // Voert een filter uit om te garanderen dat alleen admins toegang hebben tot deze endpoint.
        public async Task<IActionResult> CreateSnack([FromBody] Snacks snack)
        {
            if (!ModelState.IsValid) // Controleert of het model geldig is.
            {
                return BadRequest(ModelState); // Geeft een 400 Bad Request terug als het model ongeldig is.
            }

            var result = await _snackService.CreateSnackAsync(snack); // Roept de service aan om de nieuwe snack aan te maken.
            if (result.Contains("exists") || result.Contains("capacity")) // Controleert op foutmeldingen.
            {
                return BadRequest(result); // Geeft een 400 Bad Request terug bij een fout.
            }

            return CreatedAtAction(nameof(GetSnackById), new { id = snack.SnacksId }, snack); // Geeft een 201 Created terug met de nieuwe snack.
        }

        // PUT: api/v1/snacks/{id}
        [HttpPut("{id}")] // HTTP PUT om een bestaande snack bij te werken op basis van ID.
        [ServiceFilter(typeof(AdminAuthFilter))] // Voert een filter uit om te garanderen dat alleen admins toegang hebben tot deze endpoint.
        public async Task<IActionResult> UpdateSnack(int id, [FromBody] Snacks updatedSnack)
        {
            if (id != updatedSnack.SnacksId) // Controleert of de ID van de snack overeenkomt met de opgegeven ID.
            {
                return BadRequest("Snack ID mismatch."); // Geeft een 400 Bad Request terug als de ID's niet overeenkomen.
            }

            var result = await _snackService.UpdateSnackAsync(id, updatedSnack); // Roept de service aan om de snack bij te werken.
            if (result == "Snack not found.") // Controleert of de snack niet bestaat.
            {
                return NotFound(result); // Geeft een 404 Not Found terug als de snack niet bestaat.
            }

            return Ok(result); // Geeft de resultaat terug als een HTTP 200 OK response.
        }

        // DELETE: api/v1/snacks/{id}
        [HttpDelete("{id}")] // HTTP DELETE om een snack te verwijderen op basis van ID.
        [ServiceFilter(typeof(AdminAuthFilter))] // Voert een filter uit om te garanderen dat alleen admins toegang hebben tot deze endpoint.
        public async Task<IActionResult> DeleteSnack(int id)
        {
            var result = await _snackService.DeleteSnackAsync(id); // Roept de service aan om de snack te verwijderen.
            if (result.Contains("not found")) // Controleert of de snack niet bestaat.
            {
                return NotFound(result); // Geeft een 404 Not Found terug als de snack niet bestaat.
            }

            return Ok(result); // Geeft de resultaat terug als een HTTP 200 OK response.
        }
    }
}
