using System.Collections.Generic;
using System.Threading.Tasks;
using StarterKit.Models;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class SnackReservationService
    {
        private readonly DatabaseContext _context;

        public SnackReservationService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<int, string>> GetAllSnacksAsync()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            var snack = _context.Snacks;
            foreach (var item in snack)
            {
                dict.Add(item.SnacksId, item.Name);
            }
            return dict;
        }

        public async Task<Dictionary<int, int>> PlaceSnackOrder(int? reservationId, List<SnackOrder> snackOrders)
        {
            Dictionary<int, int> snackOrderDictionary = new Dictionary<int, int>();

            // Logic to handle reservationId
            Reservation reservation = null;

            if (reservationId.HasValue)
            {
                // Check if the reservation exists, or create a new reservation if necessary
                reservation = await _context.Reservation.FindAsync(reservationId.Value);

                if (reservation == null)
                {
                    return null;
                }
            }
            else
            {
                return new Dictionary<int, int>(); // Handle invalid or missing reservationId
            }

            // Loop through the snack orders and save them to the database
            foreach (var order in snackOrders)
            {
                if (order.id.HasValue && order.amount.HasValue)
                {
                    // Check if the snack exists
                    var snack = await _context.Snacks.FindAsync(order.id.Value);
                    if (snack != null)
                    {
                        // Create a new ReservationSnack entry
                        var reservationSnack = new ReservationSnack
                        {
                            ReservationId = reservation.ReservationId,
                            SnacksId = snack.SnacksId,
                            Amount = order.amount.Value,
                            TotalPrice = snack.Price * order.amount.Value // Assuming TotalPrice is calculated based on snack price
                        };

                        _context.ReservationSnacks.Add(reservationSnack);

                        // Add to the dictionary (for the response)
                        snackOrderDictionary[snack.SnacksId] = order.amount.Value;
                    }
                    else
                    {
                        throw new Exception($"Snack with ID {order.id.Value} does not exist.");
                    }
                }
            }

            // Save changes to the database (both ReservationSnacks and any new Reservation)
            await _context.SaveChangesAsync();

            return snackOrderDictionary;
        }


    }
}
