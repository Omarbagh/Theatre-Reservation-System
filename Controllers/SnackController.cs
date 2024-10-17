using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/v1/snacks")]
    public class SnackController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public SnackController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/v1/reservation/snacks
        [HttpGet]
        public async Task<IActionResult> GetSnacks()
        {
            var snacks = await _context.Snacks.ToListAsync();
            if (snacks.Count == 0)
            {
                return NoContent();
            }
            return Ok(snacks);
        }

        // GET: api/v1/reservation/snacks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSnackById(int id)
        {
            var snack = await _context.Snacks.FindAsync(id);
            if (snack == null)
                return NotFound();

            return Ok(snack);
        }

        // POST: api/v1/reservation/snacks
        [HttpPost]
        [ServiceFilter(typeof(AdminAuthFilter))]
        public async Task<IActionResult> CreateSnackReservation([FromBody] Snacks snack)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingSnack = await _context.Snacks.FindAsync(snack.SnacksId);
            if (existingSnack != null)
            {
                return Conflict($"A snack with ID {snack.SnacksId} already exists.");
            }
            // Check if snack capacity allows the reservation
            if (snack.Amount > snack.Capacity)
                return BadRequest("Requested amount exceeds the snack's capacity.");

            _context.Snacks.Add(snack);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSnackById), new { id = snack.SnacksId }, snack);
        }

        // PUT: api/v1/reservation/snacks/{id}
        [HttpPut("{id}")]
        [ServiceFilter(typeof(AdminAuthFilter))]
        public async Task<IActionResult> UpdateSnack(int id, [FromBody] Snacks updatedSnack)
        {
            if (id != updatedSnack.SnacksId)
                return BadRequest("Snack ID mismatch.");

            var snack = await _context.Snacks.FindAsync(id);
            if (snack == null)
                return NotFound();

            // Update snack properties
            snack.Name = updatedSnack.Name;
            snack.Capacity = updatedSnack.Capacity;
            snack.Amount = updatedSnack.Amount;
            snack.Price = updatedSnack.Price;

            await _context.SaveChangesAsync();

            return Ok(snack);
        }

        // DELETE: api/v1/reservation/snacks/{id}
        [HttpDelete("{id}")]
        // [ServiceFilter(typeof(AdminAuthFilter))]
        public async Task<IActionResult> DeleteSnack(int id)
        {
            var snack = await _context.Snacks.FindAsync(id);
            if (snack == null)
                return NotFound();

            _context.Snacks.Remove(snack);
            await _context.SaveChangesAsync();

            return Ok($"Snack with id {snack.SnacksId} deleted successfully.");
        }
    }
}