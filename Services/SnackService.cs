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

        // Methode om alle snacks op te halen
        public async Task<List<Snacks>> GetAllSnacksAsync()
        {
            return await _context.Snacks.ToListAsync(); // Haalt alle snacks op uit de database.
        }

        // Methode om een snack op te halen op basis van ID
        public async Task<Snacks> GetSnackByIdAsync(int id)
        {
            return await _context.Snacks.FindAsync(id); // Zoekt naar een specifieke snack op ID.
        }

        // Methode om een nieuwe snack aan te maken
        public async Task<string> CreateSnackAsync(Snacks newSnack)
        {
            // Controleert of een snack met hetzelfde ID al bestaat
            var existingSnack = await _context.Snacks.FindAsync(newSnack.SnacksId);
            if (existingSnack != null)
            {
                return $"A snack with ID {newSnack.SnacksId} already exists."; // Retourneert foutmelding als de snack al bestaat.
            }

            // Capaciteitscontrole
            if (newSnack.Amount > newSnack.Capacity)
            {
                return "Requested amount exceeds the snack's capacity."; // Retourneert foutmelding als de capaciteit overschreden is.
            }

            _context.Snacks.Add(newSnack); // Voegt de nieuwe snack toe aan de database.
            await _context.SaveChangesAsync(); // Slaat de wijzigingen op in de database.
            return "Snack created successfully."; // Retourneert succesmelding.
        }

        // Methode om een bestaande snack te updaten
        public async Task<string> UpdateSnackAsync(int id, Snacks updatedSnack)
        {
            // Zoekt naar de bestaande snack op basis van ID
            var existingSnack = await _context.Snacks.FindAsync(id);
            if (existingSnack == null)
            {
                return "Snack not found."; // Retourneert foutmelding als de snack niet bestaat.
            }

            // Update de eigenschappen van de snack
            existingSnack.Name = updatedSnack.Name;
            existingSnack.Capacity = updatedSnack.Capacity;
            existingSnack.Amount = updatedSnack.Amount;
            existingSnack.Price = updatedSnack.Price;

            await _context.SaveChangesAsync(); // Slaat de bijgewerkte gegevens op in de database.
            return "Snack updated successfully."; // Retourneert succesmelding.
        }

        // Methode om een snack te verwijderen
        public async Task<string> DeleteSnackAsync(int id)
        {
            // Zoekt naar de bestaande snack op basis van ID
            var existingSnack = await _context.Snacks.FindAsync(id);
            if (existingSnack == null)
            {
                return "Snack not found."; // Retourneert foutmelding als de snack niet bestaat.
            }

            _context.Snacks.Remove(existingSnack); // Verwijdert de snack uit de database.
            await _context.SaveChangesAsync(); // Slaat de wijzigingen op in de database.
            return $"Snack with ID {id} deleted successfully."; // Retourneert succesmelding.
        }
    }
}
