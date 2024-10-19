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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AdminDashboardService _adminDashboardService;

        public AdminDashboardController(IHttpContextAccessor httpContextAccessor, DatabaseContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _adminDashboardService = new AdminDashboardService(context);
        }

        [ServiceFilter(typeof(AdminAuthFilter))]
        [HttpGet]
        public async Task<IActionResult> GetAllData()
        {
            var adminData = await _adminDashboardService.GetAllDataAsync();
            return Ok(adminData);
        }

        [ServiceFilter(typeof(AdminAuthFilter))]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdminDSById(int id)
        {
            var adminData = await _adminDashboardService.GetAdminDashboardByIdAsync(id);

            if (adminData == null)
            {
                return NotFound();
            }

            return Ok(adminData);
        }
    }
}
