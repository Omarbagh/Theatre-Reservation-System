using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarterKit.Models;
using System.Linq;
using System.Threading.Tasks;

[Route("admindashboard")]
[ApiController]
public class AdminDashboardController : ControllerBase
{
    private readonly DatabaseContext _context;

    public AdminDashboardController(DatabaseContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllData()
    {
        var adminData = await _context.AdminDashboards
            .Include(ad => ad.Customer)
            .Include(ad => ad.TheatreShow)
            .Include(ad => ad.Venue)
            .ToListAsync();

        var result = adminData.Select(ad => new
        {
            ad.ReservationId,
            CustomerName = $"{ad.Customer?.FirstName} {ad.Customer?.LastName}",
            ad.AmountOfTickets,
            ad.TotalPrice,
            ShowTitle = ad.TheatreShow?.Title,
            VenueName = ad.Venue?.Name,
            DateAndTime = ad.DateAndTime,
            ad.ReservationUsed,
            SnacksDetails = ad.SnacksDetails
        });

        return new JsonResult(result);
    }
}
