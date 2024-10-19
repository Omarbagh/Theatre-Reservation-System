using Microsoft.EntityFrameworkCore;
using StarterKit.Models;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly DatabaseContext _context;

    public AdminDashboardService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<AdminDashboard>> GetAllDataAsync()
    {
        return await _context.AdminDashboards
            .ToListAsync();
    }

    public async Task<AdminDashboard?> GetAdminDashboardByIdAsync(int id)
    {
        return await _context.AdminDashboards
            .FirstOrDefaultAsync(ad => ad.ReservationId == id);
    }
}
