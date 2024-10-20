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

        // Methode om alle snacks op te halen als een dictionary (SnackId, SnackNaam)
        public async Task<Dictionary<int, string>> GetAllSnacksAsync()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>(); // Dictionary om snacks op te slaan.
            var snack = _context.Snacks; // Haalt alle snacks op uit de database.
            foreach (var item in snack)
            {
                dict.Add(item.SnacksId, item.Name); // Voegt elke snack toe aan de dictionary.
            }
            return dict;
        }

        // Methode om een snackbestelling te plaatsen voor een specifieke reservatie
        public async Task<Dictionary<int, int>> PlaceSnackOrder(int? reservationId, List<SnackOrder> snackOrders)
        {
            Dictionary<int, int> snackOrderDictionary = new Dictionary<int, int>(); // Dictionary om de bestelde snacks en hoeveelheden op te slaan.

            // Logica om de reservatie te behandelen
            Reservation reservation = null;

            if (reservationId.HasValue)
            {
                // Controleert of de reservatie bestaat
                reservation = await _context.Reservation.FindAsync(reservationId.Value);

                if (reservation == null)
                {
                    return null; // Retourneert null als de reservatie niet bestaat.
                }
            }
            else
            {
                return new Dictionary<int, int>(); // Retourneert een lege dictionary als reservationId niet geldig is.
            }

            // Doorloopt de snackbestellingen en slaat ze op in de database
            foreach (var order in snackOrders)
            {
                if (order.id.HasValue && order.amount.HasValue)
                {
                    // Controleert of de snack bestaat
                    var snack = await _context.Snacks.FindAsync(order.id.Value);
                    if (snack != null)
                    {
                        // Maakt een nieuwe ReservationSnack aan
                        var reservationSnack = new ReservationSnack
                        {
                            ReservationId = reservation.ReservationId,
                            SnacksId = snack.SnacksId,
                            Amount = order.amount.Value,
                            TotalPrice = snack.Price * order.amount.Value // Berekent de totale prijs op basis van de snackprijs.
                        };

                        _context.ReservationSnacks.Add(reservationSnack); // Voegt de ReservationSnack toe aan de database.

                        // Voegt de bestelde snack en hoeveelheid toe aan de dictionary
                        snackOrderDictionary[snack.SnacksId] = order.amount.Value;
                    }
                    else
                    {
                        throw new Exception($"Snack with ID {order.id.Value} does not exist."); // Gooi een foutmelding als de snack niet bestaat.
                    }
                }
            }

            // Slaat wijzigingen op in de database (zowel ReservationSnacks als nieuwe Reservation)
            await _context.SaveChangesAsync();

            return snackOrderDictionary; // Retourneert de dictionary met de bestelde snacks.
        }


    }
}
