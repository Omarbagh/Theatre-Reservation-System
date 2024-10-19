using StarterKit.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public interface IReservationService
{
    Task<ReservationResponseDto> CreateReservationAsync(ReservationDto reservationDto);
}

public class ReservationService : IReservationService
{
    private readonly DatabaseContext _context;

    public ReservationService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<ReservationResponseDto> CreateReservationAsync(ReservationDto reservationDto)
    {
        var theatreShowDate = await _context.TheatreShowDate
            .Include(ts => ts.TheatreShow)
                .ThenInclude(t => t.Venue)
            .FirstOrDefaultAsync(ts => ts.TheatreShowDateId == reservationDto.ShowDateId);

        if (theatreShowDate == null)
        {
            throw new KeyNotFoundException($"The show date with ID {reservationDto.ShowDateId} was not found.");
        }

        var customer = new Customer
        {
            FirstName = reservationDto.FirstName,
            LastName = reservationDto.LastName,
            Email = reservationDto.Email
        };

        var existingCustomer = await _context.Customer
    .FirstOrDefaultAsync(c => c.Email == customer.Email);

        if (existingCustomer == null)
        {
            // De klant bestaat nog niet, dus voeg deze toe
            _context.Customer.Add(customer);
        }
        else
        {
            // De klant bestaat al, gebruik het bestaande klantobject
            customer = existingCustomer;
        }

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

            return new ReservationResponseDto
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
                    TheatreShowDateId = theatreShowDate.TheatreShowDateId,
                    DateAndTime = theatreShowDate.DateAndTime
                }
            };
        }
        else
        {
            throw new KeyNotFoundException("The associated venue for the theatre show was not found.");
        }
    }
}
