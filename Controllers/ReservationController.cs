using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[Route("api/v1/Reservation")]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly DatabaseContext _context;

    public ReservationController(DatabaseContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationDto reservationDto)
    {
        if (ModelState.IsValid)
        {
            var theatreShowDate = await _context.TheatreShowDate
                .Include(ts => ts.TheatreShow)
                    .ThenInclude(t => t.Venue)
                .FirstOrDefaultAsync(ts => ts.TheatreShowDateId == reservationDto.ShowDateId);

            if (theatreShowDate == null)
            {
                return NotFound($"The show date with ID {reservationDto.ShowDateId} was not found.");
            }

            var customer = new Customer
            {
                FirstName = reservationDto.FirstName,
                LastName = reservationDto.LastName,
                Email = reservationDto.Email
            };

            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                AmountOfTickets = reservationDto.AmountOfTickets,
                Used = false,
                CustomerId = customer.CustomerId,
                TheatreShowDateId = theatreShowDate.TheatreShowDateId
            };

            if (theatreShowDate.TheatreShow.Venue != null)
            {
                var adminDashboardEntry = new AdminDashboard
                {
                    CustomerId = customer.CustomerId,
                    TheatreShowId = theatreShowDate.TheatreShow.TheatreShowId,
                    VenueId = theatreShowDate.TheatreShow.Venue.VenueId,
                    AmountOfTickets = reservation.AmountOfTickets,
                    TotalPrice = reservation.AmountOfTickets * (decimal)theatreShowDate.TheatreShow.Price,
                    SnacksDetails = "N/A",
                    DateAndTime = DateTime.UtcNow,
                    ReservationUsed = false
                };

                _context.AdminDashboards.Add(adminDashboardEntry);
                _context.Reservation.Add(reservation);

                await _context.SaveChangesAsync();

                var result = new ReservationResponseDto
                {
                    ReservationId = reservation.ReservationId,
                    AmountOfTickets = reservation.AmountOfTickets,
                    Used = reservation.Used,
                    Customer = new CustomerDto
                    {
                        CustomerId = customer.CustomerId,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Email = customer.Email
                    },
                    TheatreShowDate = new TheatreShowDateDto
                    {
                        TheatreShowDateId = reservation.TheatreShowDate.TheatreShow.TheatreShowId,
                        DateAndTime = reservation.TheatreShowDate.DateAndTime
                    }
                };

                return CreatedAtAction(nameof(CreateReservation), new { id = reservation.ReservationId }, result);
            }
            else
            {
                return NotFound("The associated venue for the theatre show was not found.");
            }
        }

        return BadRequest(ModelState);
    }



    // DTO for creating a reservation
    public class ReservationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int ShowDateId { get; set; }
        public int AmountOfTickets { get; set; }
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
    }
}
