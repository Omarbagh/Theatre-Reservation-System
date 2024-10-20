using Microsoft.EntityFrameworkCore;
using StarterKit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class ReservationManagementService
    {
        private readonly DatabaseContext _context;

        public ReservationManagementService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<ReservationResponseDto>> GetReservations(int? showId, DateTime? date)
        {
            var query = _context.Reservation
                .Include(r => r.Customer) // Voegt klantgegevens toe aan de query.
                .Include(r => r.TheatreShowDate)
                    .ThenInclude(tsd => tsd.TheatreShow) // Voegt theatervoorstellingsgegevens toe.
                .AsQueryable(); // Start de query als doorzoekbaar.

            if (showId.HasValue)
            {
                query = query.Where(r => r.TheatreShowDate.TheatreShow.TheatreShowId == showId.Value); // Filtert op showId als deze is opgegeven.
            }

            if (date.HasValue)
            {
                query = query.Where(r => r.TheatreShowDate.DateAndTime.Date == date.Value.Date); // Filtert op datum als deze is opgegeven.
            }

            var reservations = await query.ToListAsync(); // Voert de query uit en haalt de resultaten op.

            // Maakt een lijst van DTO's (Data Transfer Objects) om de reserveringen terug te geven.
            return reservations.Select(r => new ReservationResponseDto
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
        }

        public async Task<List<ReservationResponseDto>> SearchReservations(string email, int? reservationId)
        {
            var query = _context.Reservation
                .Include(r => r.Customer) // Voegt klantgegevens toe aan de query.
                .Include(r => r.TheatreShowDate)
                    .ThenInclude(tsd => tsd.TheatreShow) // Voegt theatervoorstellingsgegevens toe.
                .AsQueryable(); // Start de query als doorzoekbaar.

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(r => r.Customer.Email.Contains(email)); // Filtert op e-mail als deze is opgegeven.
            }

            if (reservationId.HasValue)
            {
                query = query.Where(r => r.ReservationId == reservationId.Value); // Filtert op reserverings-ID als deze is opgegeven.
            }

            var reservations = await query.ToListAsync(); // Voert de query uit en haalt de resultaten op.

            // Maakt een lijst van DTO's (Data Transfer Objects) om de reserveringen terug te geven.
            return reservations.Select(r => new ReservationResponseDto
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
        }

        public async Task<string> MarkReservationAsUsed(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id); // Zoekt de reservering op basis van ID.

            if (reservation == null)
            {
                return $"Reservation with ID {id} not found."; // Geeft een foutmelding terug als de reservering niet bestaat.
            }

            reservation.Used = true; // Markeert de reservering als gebruikt.
            var adminDashboard = await _context.AdminDashboards
                    .FirstOrDefaultAsync(ad => ad.ReservationId == id); // Zoekt het dashboard van de admin op basis van reserverings-ID.
            if (adminDashboard != null)
            {
                adminDashboard.ReservationUsed = true;
            }
            await _context.SaveChangesAsync(); // Slaat de wijzigingen op in de database.

            return $"Reservation with ID {id} has been marked as used."; // Geeft een succesmelding terug.
        }

        public async Task<string> DeleteReservation(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id); // Zoekt de reservering op basis van ID.

            if (reservation == null)
            {
                return $"Reservation with ID {id} not found."; // Geeft een foutmelding terug als de reservering niet bestaat.
            }

            _context.Reservation.Remove(reservation); // Verwijdert de reservering uit de database.
            await _context.SaveChangesAsync(); // Slaat de wijzigingen op in de database.

            return $"Reservation with ID {id} has been deleted."; // Geeft een succesmelding terug.
        }
    }
}
