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
    public async Task<IActionResult> CreateReservations([FromBody] List<ReservationDto> reservationDtos)
    {
        if (reservationDtos == null || reservationDtos.Count == 0)
        {
            return BadRequest("No reservations provided.");
        }

        var results = new List<ReservationResponseDto>();

        foreach (var reservationDto in reservationDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var theatreShowDate = await _context.TheatreShowDate.FindAsync(reservationDto.ShowDateId);
            if (theatreShowDate == null)
            {
                return NotFound($"The show date with ID {reservationDto.ShowDateId} was not found.");
            }

            var amountoftickets = reservationDto.AmountOfTickets;
            if (amountoftickets == 0)
            {
                return NotFound($"You can not book 0 tickets.");
            }

            var customer = new Customer
            {
                FirstName = reservationDto.FirstName,
                LastName = reservationDto.LastName,
                Email = reservationDto.Email
            };

            var reservation = new Reservation
            {
                AmountOfTickets = reservationDto.AmountOfTickets,
                Used = false,
                Customer = customer,
                TheatreShowDate = theatreShowDate
            };

            _context.Reservation.Add(reservation);
            await _context.SaveChangesAsync();

            var result = new ReservationResponseDto
            {
                ReservationId = reservation.ReservationId,
                AmountOfTickets = reservation.AmountOfTickets,
                Used = reservation.Used,
                Customer = new CustomerDto
                {
                    CustomerId = reservation.Customer.CustomerId,
                    FirstName = reservation.Customer.FirstName,
                    LastName = reservation.Customer.LastName,
                    Email = reservation.Customer.Email
                },
                TheatreShowDate = new TheatreShowDateDto
                {
                    TheatreShowDateId = reservation.TheatreShowDate.TheatreShowDateId,
                    DateAndTime = reservation.TheatreShowDate.DateAndTime
                }
            };

            results.Add(result);
        }

        return CreatedAtAction(nameof(CreateReservations), results);
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
