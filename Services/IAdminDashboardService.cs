using System.Collections.Generic;
using System.Threading.Tasks;
using StarterKit.Models;

public interface IAdminDashboardService
{
    Task<List<AdminDashboard>> GetAllDataAsync();
    Task<AdminDashboard?> GetAdminDashboardByIdAsync(int id);
}

