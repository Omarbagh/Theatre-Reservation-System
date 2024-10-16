using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using StarterKit.Models;
using Microsoft.EntityFrameworkCore;

using System.Text.Json;

public class AdminDashbordController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DatabaseContext _context;

    public AdminDashbordController(IHttpContextAccessor httpContextAccessor, DatabaseContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllData()
    {
        var adminData = await _context.AdminDashboards
            .Include(ad => ad.ReservationId)
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

        return Ok(result);
    }



}