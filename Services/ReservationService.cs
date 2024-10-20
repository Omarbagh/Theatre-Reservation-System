using StarterKit.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public interface IReservationService
{
    Task<ReservationResponseDto> CreateReservationAsync(ReservationDto reservationDto, List<SnackOrder> snackOrders);
}

public class ReservationService : IReservationService
{
    private readonly DatabaseContext _context;

    public ReservationService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<ReservationResponseDto> CreateReservationAsync(ReservationDto reservationDto, List<SnackOrder> snackOrders)
    {
        // Haalt de voorstelling datum op
        var theatreShowDate = await _context.TheatreShowDate
            .Include(ts => ts.TheatreShow)
                .ThenInclude(t => t.Venue)
            .FirstOrDefaultAsync(ts => ts.TheatreShowDateId == reservationDto.ShowDateId);

        if (theatreShowDate == null)
        {
            throw new KeyNotFoundException($"The show date with ID {reservationDto.ShowDateId} was not found.");
        }

        // Creëert een nieuw klantobject
        var customer = new Customer
        {
            FirstName = reservationDto.FirstName,
            LastName = reservationDto.LastName,
            Email = reservationDto.Email
        };

        // Controleert of de klant al bestaat
        var existingCustomer = await _context.Customer
            .FirstOrDefaultAsync(c => c.Email == customer.Email);

        if (existingCustomer == null)
        {
            // Voeg nieuwe klant toe
            _context.Customer.Add(customer);
        }
        else
        {
            // Gebruik bestaande klant
            customer = existingCustomer;
        }

        await _context.SaveChangesAsync();

        // Creëert een nieuwe reservering
        var reservation = new Reservation
        {
            AmountOfTickets = reservationDto.AmountOfTickets,
            Used = false,
            CustomerId = customer.CustomerId,
            TheatreShowDateId = theatreShowDate.TheatreShowDateId
        };

        // Voeg de reservering toe aan de context
        _context.Reservation.Add(reservation);
        await _context.SaveChangesAsync(); // Sla op om de ReservationId te krijgen

        // Creëert een variabele om de totale prijs van snacks bij te houden
        decimal totalSnackPrice = 0;

        // Haalt de details van de snacks op en berekent de totale prijs
        var snackDetailsList = new List<string>(); // Maak een nieuwe lijst om snackdetails op te slaan
        foreach (var so in snackOrders)
        {
            var snack = await _context.Snacks.FindAsync(so.id); // Gebruik SnackId van SnackOrder
            if (snack != null)
            {
                // Voeg de naam en hoeveelheid van de snack toe aan de lijst
                snackDetailsList.Add($"{snack.Name} (x{so.amount})");
                totalSnackPrice += (int)snack.Price * (int)so.amount;
            }
        }

        // Voegt de snackdetails samen tot een enkele string
        string snackDetails = string.Join(", ", snackDetailsList);

        // Voeg de totale prijs van de snacks toe aan de snackdetails
        snackDetails += $" | Total Snack Price: {totalSnackPrice:C}"; // Formatteert de totale prijs als valuta

        if (theatreShowDate.TheatreShow.Venue != null)
        {
            // Creëert een nieuw AdminDashboard entry met alle relevante gegevens van de reservering en show.
            var adminDashboardEntry = new AdminDashboard
            {
                CustomerId = customer.CustomerId,
                TheatreShowId = theatreShowDate.TheatreShow.TheatreShowId,
                VenueId = theatreShowDate.TheatreShow.Venue.VenueId,
                AmountOfTickets = reservation.AmountOfTickets,
                TotalPrice = reservation.AmountOfTickets * (decimal)theatreShowDate.TheatreShow.Price + totalSnackPrice, // Inclusief de totale prijs van snacks
                SnacksDetails = snackDetails,
                DateAndTime = DateTime.UtcNow,
                ReservationUsed = false
            };

            _context.AdminDashboards.Add(adminDashboardEntry);

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
