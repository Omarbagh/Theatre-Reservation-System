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
        [HttpGet]
        public async Task<IActionResult> GetSnacks()
        {
            var snacks = await _snackService.GetAllSnacksAsync();
            if (snacks.Count == 0)
            {
                return NoContent();
            }
            return Ok(snacks);
        }

        // GET: api/v1/snacks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSnackById(int id)
        {
            var snack = await _snackService.GetSnackByIdAsync(id);
            if (snack == null)
            {
                return NotFound();
            }
            return Ok(snack);
        }

        // POST: api/v1/snacks
        [HttpPost]
        [ServiceFilter(typeof(AdminAuthFilter))]
        public async Task<IActionResult> CreateSnack([FromBody] Snacks snack)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _snackService.CreateSnackAsync(snack);
            if (result.Contains("exists") || result.Contains("capacity"))
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetSnackById), new { id = snack.SnacksId }, snack);
        }

        // PUT: api/v1/snacks/{id}
        [HttpPut("{id}")]
        [ServiceFilter(typeof(AdminAuthFilter))]
        public async Task<IActionResult> UpdateSnack(int id, [FromBody] Snacks updatedSnack)
        {
            if (id != updatedSnack.SnacksId)
            {
                return BadRequest("Snack ID mismatch.");
            }

            var result = await _snackService.UpdateSnackAsync(id, updatedSnack);
            if (result == "Snack not found.")
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        // DELETE: api/v1/snacks/{id}
        [HttpDelete("{id}")]
        [ServiceFilter(typeof(AdminAuthFilter))]
        public async Task<IActionResult> DeleteSnack(int id)
        {
            var result = await _snackService.DeleteSnackAsync(id);
            if (result.Contains("not found"))
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
