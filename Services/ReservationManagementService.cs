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
                .Include(r => r.Customer)
                .Include(r => r.TheatreShowDate)
                    .ThenInclude(tsd => tsd.TheatreShow)
                .AsQueryable();

            if (showId.HasValue)
            {
                query = query.Where(r => r.TheatreShowDate.TheatreShow.TheatreShowId == showId.Value);
            }

            if (date.HasValue)
            {
                query = query.Where(r => r.TheatreShowDate.DateAndTime.Date == date.Value.Date);
            }

            var reservations = await query.ToListAsync();

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

            var reservations = await query.ToListAsync();

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
            var reservation = await _context.Reservation.FindAsync(id);

            if (reservation == null)
            {
                return $"Reservation with ID {id} not found.";
            }

            reservation.Used = true;
            await _context.SaveChangesAsync();

            return $"Reservation with ID {id} has been marked as used.";
        }

        public async Task<string> DeleteReservation(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);

            if (reservation == null)
            {
                return $"Reservation with ID {id} not found.";
            }

            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();

            return $"Reservation with ID {id} has been deleted.";
        }
    }
}
