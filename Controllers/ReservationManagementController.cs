using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarterKit.Models;


[Route("api/v1/admin/reservations")]
public class ReservationManagementController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReservationManagementController(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    public async Task<IActionResult> GetReservations()
    {

        // Check if admin is logged in
        var isAdminLoggedInResult = IsAdminLoggedIn();
        if (!isAdminLoggedInResult)
        {
            return Unauthorized("You must be logged in as an admin to view reservations.");
        }

        var reservations = await _context.Reservation
            .Select(r => new ReservationResponseDto
            {
                ReservationId = r.ReservationId,
                AmountOfTickets = r.AmountOfTickets,
                Used = r.Used,
                Customer = new CustomerDto
                {
                    CustomerId = r.Customer.CustomerId,
                    FirstName = r.Customer.FirstName,
                    LastName = r.Customer.LastName,
                    Email = r.Customer.Email
                },
                TheatreShowDate = new TheatreShowDateDto
                {
                    TheatreShowDateId = r.TheatreShowDate.TheatreShowDateId,
                    DateAndTime = r.TheatreShowDate.DateAndTime,
                    TheatreShow = new TheatreShowDto
                    {
                        TheatreShowId = r.TheatreShowDate.TheatreShow.TheatreShowId,
                        Title = r.TheatreShowDate.TheatreShow.Title,
                        Description = r.TheatreShowDate.TheatreShow.Description,
                        Price = r.TheatreShowDate.TheatreShow.Price
                    }
                }
            })
            .ToListAsync();
        if (reservations == null || reservations.Count == 0)
        {
            return NotFound("No reservations found.");
        }

        return Ok(reservations);
    }

    [HttpDelete("DeleteReservation/{id}")]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var adminCheckResult = IsAdminLoggedIn();

        if (!adminCheckResult)
        {
            return Unauthorized("Only admins can Delete reservation(s).");
        }

        // Find the show by id
        var reservation = await _context.Reservation.FirstOrDefaultAsync(r => r.ReservationId == id);
        if (reservation == null)
        {
            return NotFound("Reservation not found.");
        }
        //lala
        try
        {
            // Remove the reservation
            _context.Reservation.Remove(reservation);

            await _context.SaveChangesAsync();
            return Ok("Reservation deleted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the show: {ex.Message}");
        }
    }

    // DTO classes for response
    public class ReservationResponseDto
    {
        public int ReservationId { get; set; }
        public int AmountOfTickets { get; set; }
        public bool Used { get; set; }
        public CustomerDto Customer { get; set; }
        public TheatreShowDateDto TheatreShowDate { get; set; }
    }

    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class TheatreShowDateDto
    {
        public int TheatreShowDateId { get; set; }
        public DateTime DateAndTime { get; set; }
        public TheatreShowDto TheatreShow { get; set; }
    }

    public class TheatreShowDto
    {
        public int TheatreShowId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }

    private bool IsAdminLoggedIn()
    {
        var username = _httpContextAccessor.HttpContext?.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            return false;
        }

        var admin = _context.Admin
            .Where(a => a.UserName == username)
            .Select(a => a.UserName)
            .FirstOrDefault();

        return admin != null;
    }
}