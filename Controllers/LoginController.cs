using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Services;
using StarterKit.Models;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Session;


namespace StarterKit.Controllers;


[Route("api/v1/Login")]
public class LoginController : Controller
{
    private readonly ILoginService _loginService;

    private readonly DatabaseContext _context;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginController(ILoginService loginService, DatabaseContext context, IHttpContextAccessor httpContextAccessor)
    {
        _loginService = loginService;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }


    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginBody loginBody)
    {
        var loginStatus = _loginService.CheckPassword(loginBody.Username, loginBody.Password);

        switch (loginStatus)
        {
            case LoginStatus.Success:
                HttpContext.Session.SetString("Username", loginBody.Username);
                return Ok(new { message = "Login successful" });  // Ensure JSON response

            case LoginStatus.IncorrectUsername:
                return Unauthorized(new { message = "Incorrect username" });

            case LoginStatus.IncorrectPassword:
            default:
                return Unauthorized(new { message = "Incorrect password" });
        }
    }


    [HttpGet("IsAdminLoggedIn")]
    public async Task<IActionResult> IsAdminLoggedIn()
    {

        // Haal de gebruikersnaam op uit de HTTP-context (sessie).
        var username = HttpContext.Session.GetString("Username");

        // Controleer of de gebruikersnaam leeg of null is.
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized(new { IsLoggedIn = false }); // Geeft een 401 Unauthorized terug als de admin niet is ingelogd.
        }

        // Gebruik LINQ om de database te doorzoeken en de admin op te halen op basis van de gebruikersnaam.
        var admin = await _context.Admin
            .Where(a => a.UserName == username) // Filtert op gebruikersnaam.
            .Select(a => a.UserName) // Selecteert de gebruikersnaam van de admin.
            .FirstOrDefaultAsync(); // Haalt de eerste overeenkomende admin op of null als er geen is.

        // Controleer of de admin is gevonden.
        if (admin != null)
        {
            return Ok(new { IsLoggedIn = true, AdminName = admin }); // Geeft een 200 OK response terug met inlogstatus en adminnaam.
        }

        return Unauthorized(new { IsLoggedIn = false }); // Geeft een 401 Unauthorized terug als de admin niet is gevonden.
    }

    [HttpGet("Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok("Logged out");
    }

}

public class LoginBody
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}