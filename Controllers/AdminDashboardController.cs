using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/v1/admindashboard")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor; // Toegang tot de HTTP-context.
        private readonly AdminDashboardService _adminDashboardService;

        public AdminDashboardController(IHttpContextAccessor httpContextAccessor, DatabaseContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _adminDashboardService = new AdminDashboardService(context);
        }

        // Endpoint: GET http://localhost:5097/api/v1/admindashboard
        [ServiceFilter(typeof(AdminAuthFilter))] // Voert een filter uit om te garanderen dat alleen admins toegang hebben tot deze endpoints.
        [HttpGet] // HTTP GET om alle gegevens van het admin dashboard op te halen.
        public async Task<IActionResult> GetAllData()
        {
            var adminData = await _adminDashboardService.GetAllDataAsync(); // Haalt alle admin dashboard gegevens op asynchroon.
            return Ok(adminData); // Geeft de admin gegevens terug als een HTTP 200 OK response.
        }

        // Endpoint: GET http://localhost:5097/api/v1/admindashboard/{id}
        [ServiceFilter(typeof(AdminAuthFilter))]
        [HttpGet("{id}")]// HTTP GET om specifieke admin dashboard gegevens op te halen op basis van ID.
        public async Task<IActionResult> GetAdminDSById(int id)
        {
            var adminData = await _adminDashboardService.GetAdminDashboardByIdAsync(id);  // Haalt admin gegevens op op basis van het opgegeven ID.

            if (adminData == null)
            {
                return NotFound(); // Geeft een 404 Not Found terug als de admin gegevens niet bestaan.
            }

            return Ok(adminData); // Geeft de admin gegevens terug als een HTTP 200 OK response.
        }
    }
}
