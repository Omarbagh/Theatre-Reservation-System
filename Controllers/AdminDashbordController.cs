using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using StarterKit.Models;
using Microsoft.EntityFrameworkCore;

using System.Text.Json;
namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/v1/admindashbord")]
    public class AdminDashbordController : ControllerBase
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DatabaseContext _context;

        public AdminDashbordController(IHttpContextAccessor httpContextAccessor, DatabaseContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllData()
        {
            var adminData = await _context.AdminDashboards
                .Select(ad => new
                {
                    ad.ReservationId,
                    ad.CustomerId,
                    ad.TheatreShowId,
                    ad.VenueId,
                    ad.AmountOfTickets,
                    ad.TotalPrice,
                    ad.SnacksDetails,
                    ad.DateAndTime,
                    ad.ReservationUsed
                })
                .ToListAsync();

            return Ok(adminData);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdminDSById(int id)
        {
            var adminds = await _context.AdminDashboards
                .Where(ad => ad.ReservationId == id)
                .Select(ad => new
                {
                    ad.ReservationId,
                    ad.CustomerId,
                    ad.TheatreShowId,
                    ad.VenueId,
                    ad.AmountOfTickets,
                    ad.TotalPrice,
                    ad.SnacksDetails,
                    ad.DateAndTime,
                    ad.ReservationUsed
                })
                .FirstOrDefaultAsync();

            if (adminds == null)
            {
                return NotFound();
            }

            return Ok(adminds);
        }



    }
}

