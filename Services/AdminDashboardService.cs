// Importeert de benodigde namespaces voor Entity Framework Core en de modellen van het project.
using Microsoft.EntityFrameworkCore;
using StarterKit.Models;

// Definieert een serviceklasse die de functionaliteit van het AdminDashboard afhandelt.
// De klasse implementeert de interface IAdminDashboardService.
public class AdminDashboardService : IAdminDashboardService
{
    // Definieert een privé variabele die de databasecontext opslaat.
    // Hiermee wordt toegang verkregen tot de database.
    private readonly DatabaseContext _context;

    // Constructor van de AdminDashboardService klasse.
    // De constructor neemt een DatabaseContext als parameter en wijst deze toe aan de privé variabele.
    public AdminDashboardService(DatabaseContext context)
    {
        _context = context;
    }

    // Methode die asynchroon een lijst van alle AdminDashboard records uit de database ophaalt.
    public async Task<List<AdminDashboard>> GetAllDataAsync()
    {
        // Haalt alle records van de AdminDashboards tabel op en retourneert deze als een lijst.
        return await _context.AdminDashboards
            .ToListAsync();
    }

    // Methode die asynchroon een specifiek AdminDashboard record ophaalt op basis van het opgegeven id.
    public async Task<AdminDashboard?> GetAdminDashboardByIdAsync(int id)
    {
        // Zoekt naar het eerste record in de AdminDashboards tabel waarvan de ReservationId overeenkomt met het opgegeven id.
        return await _context.AdminDashboards
            .FirstOrDefaultAsync(ad => ad.ReservationId == id);
    }
}
