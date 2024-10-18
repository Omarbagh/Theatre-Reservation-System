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

        public async Task<Dictionary<int, int>> PlaceSnackOrder(List<SnackOrder> snackOrders)
        {
            Dictionary<int, int> snackOrderDictionary = new Dictionary<int, int>();

            foreach (var order in snackOrders)
            {
                if (order.id.HasValue && order.amount.HasValue)
                {
                    // Add to the dictionary
                    snackOrderDictionary[order.id.Value] = order.amount.Value;
                }
            }

            // Optionally, save these orders to the database or perform further logic

            return snackOrderDictionary;
        }
    }
}
