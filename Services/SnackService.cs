using System.Collections.Generic;
using System.Threading.Tasks;
using StarterKit.Models;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class SnackService
    {
        private readonly DatabaseContext _context;

        public SnackService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<Snacks>> GetAllSnacksAsync()
        {
            return await _context.Snacks.ToListAsync();
        }

        public async Task<Snacks> GetSnackByIdAsync(int id)
        {
            return await _context.Snacks.FindAsync(id);
        }

        public async Task<string> CreateSnackAsync(Snacks newSnack)
        {
            var existingSnack = await _context.Snacks.FindAsync(newSnack.SnacksId);
            if (existingSnack != null)
            {
                return $"A snack with ID {newSnack.SnacksId} already exists.";
            }

            // Capacity check
            if (newSnack.Amount > newSnack.Capacity)
            {
                return "Requested amount exceeds the snack's capacity.";
            }

            _context.Snacks.Add(newSnack);
            await _context.SaveChangesAsync();
            return "Snack created successfully.";
        }

        public async Task<string> UpdateSnackAsync(int id, Snacks updatedSnack)
        {
            var existingSnack = await _context.Snacks.FindAsync(id);
            if (existingSnack == null)
            {
                return "Snack not found.";
            }

            // Update properties
            existingSnack.Name = updatedSnack.Name;
            existingSnack.Capacity = updatedSnack.Capacity;
            existingSnack.Amount = updatedSnack.Amount;
            existingSnack.Price = updatedSnack.Price;

            await _context.SaveChangesAsync();
            return "Snack updated successfully.";
        }

        public async Task<string> DeleteSnackAsync(int id)
        {
            var existingSnack = await _context.Snacks.FindAsync(id);
            if (existingSnack == null)
            {
                return "Snack not found.";
            }

            _context.Snacks.Remove(existingSnack);
            await _context.SaveChangesAsync();
            return $"Snack with ID {id} deleted successfully.";
        }
    }
}
