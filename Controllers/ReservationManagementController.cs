using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarterKit.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StarterKit.Controllers
{
    [Route("api/v1/admin/reservations")]
    [ApiController]
    [ServiceFilter(typeof(AdminAuthFilter))]
    public class ReservationManagementController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ReservationManagementController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetReservations([FromQuery] int? showId, [FromQuery] DateTime? date)
        {
            var query = _context.Reservation
                .Include(r => r.Customer)
                .Include(r => r.TheatreShowDate)
                    .ThenInclude(tsd => tsd.TheatreShow)
                .AsQueryable();

            // Optional filtering by showId
            if (showId.HasValue)
            {
                query = query.Where(r => r.TheatreShowDate.TheatreShow.TheatreShowId == showId.Value);
            }

            // Optional filtering by date
            if (date.HasValue)
            {
                query = query.Where(r => r.TheatreShowDate.DateAndTime.Date == date.Value.Date);
            }

            var reservations = await query.ToListAsync();
            // Transform the reservations into DTOs for the response
            var reservationDtos = reservations.Select(r => new ReservationResponseDto
            {
                ReservationId = r.ReservationId,
                AmountOfTickets = r.AmountOfTickets,
                Used = r.Used,
                Customer = new CustomerDto
                {
                    CustomerId = r.Customer.CustomerId,
                    FirstName = r.Customer.FirstName,
                    LastName = r.Customer.LastName,
                    Email = r.Customer.Email
                },
                TheatreShowDate = new TheatreShowDateDto
                {
                    TheatreShowDateId = r.TheatreShowDate.TheatreShowDateId,
                    DateAndTime = r.TheatreShowDate.DateAndTime,
                    TheatreShow = new TheatreShowDto
                    {
                        TheatreShowId = r.TheatreShowDate.TheatreShow.TheatreShowId,
                        Title = r.TheatreShowDate.TheatreShow.Title,
                        Description = r.TheatreShowDate.TheatreShow.Description,
                        Price = (decimal)r.TheatreShowDate.TheatreShow.Price
                    }
                }
            }).ToList();

            return Ok(reservationDtos);
        }



        // GET: api/v1/admin/reservations/search
        [HttpGet("search")]
        public async Task<IActionResult> SearchReservations([FromQuery] string email, [FromQuery] int? reservationId)
        {
            var query = _context.Reservation
                .Include(r => r.Customer)
                .Include(r => r.TheatreShowDate)
                    .ThenInclude(tsd => tsd.TheatreShow)
                .AsQueryable();

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(r => r.Customer.Email.Contains(email));
            }

            if (reservationId.HasValue)
            {
                query = query.Where(r => r.ReservationId == reservationId.Value);
            }
            // ...

            var reservations = await query
                .ToListAsync();

            if (!reservations.Any())
            {
                return NotFound("No reservations matching the search criteria were found.");
            }

            var reservationDtos = reservations.Select(r => new ReservationResponseDto
            {
                ReservationId = r.ReservationId,
                AmountOfTickets = r.AmountOfTickets,
                Used = r.Used,
                Customer = new CustomerDto
                {
                    CustomerId = r.Customer.CustomerId,
                    FirstName = r.Customer.FirstName,
                    LastName = r.Customer.LastName,
                    Email = r.Customer.Email
                },
                TheatreShowDate = new TheatreShowDateDto
                {
                    TheatreShowDateId = r.TheatreShowDate.TheatreShowDateId,
                    DateAndTime = r.TheatreShowDate.DateAndTime,
                    TheatreShow = new TheatreShowDto
                    {
                        TheatreShowId = r.TheatreShowDate.TheatreShow.TheatreShowId,
                        Title = r.TheatreShowDate.TheatreShow.Title,
                        Description = r.TheatreShowDate.TheatreShow.Description,
                        Price = (decimal)r.TheatreShowDate.TheatreShow.Price
                    }
                }
            }).ToList();

            return Ok(reservationDtos);
        }

        // PUT: api/v1/admin/reservations/{id}/mark-used
        [HttpPut("{id}/mark-used")]
        public async Task<IActionResult> MarkReservationAsUsed(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);

            if (reservation == null)
            {
                return NotFound($"Reservation with ID {id} not found.");
            }

            reservation.Used = true;
            await _context.SaveChangesAsync();

            return Ok($"Reservation with ID {id} has been marked as used.");
        }

        // DELETE: api/v1/admin/reservations/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);

            if (reservation == null)
            {
                return NotFound($"Reservation with ID {id} not found.");
            }

            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();

            return Ok($"Reservation with ID {id} has been deleted.");
        }
    }
    // DTO for returning reservation data
    public class ReservationResponseDto
    {
        public int ReservationId { get; set; }
        public int AmountOfTickets { get; set; }
        public bool Used { get; set; }

        public CustomerDto Customer { get; set; }
        public TheatreShowDateDto TheatreShowDate { get; set; }
    }
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class TheatreShowDateDto
    {
        public int TheatreShowDateId { get; set; }
        public DateTime DateAndTime { get; set; }
        public TheatreShowDto TheatreShow { get; set; }
    }

    public class TheatreShowDto
    {
        public int TheatreShowId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}