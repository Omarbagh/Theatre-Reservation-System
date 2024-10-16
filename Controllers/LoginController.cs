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
                return Ok("Login successful");

            case LoginStatus.IncorrectUsername:
                return Unauthorized("Incorrect username");

            case LoginStatus.IncorrectPassword:
            default:
                return Unauthorized("Incorrect password");
        }
    }

    [HttpGet("IsAdminLoggedIn")]
    public async Task<IActionResult> IsAdminLoggedIn()
    {

        // Get the username from the HTTP context directly
        var username = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized(new { IsLoggedIn = false });
        }

        // Use LINQ to query the database
        var admin = await _context.Admin
            .Where(a => a.UserName == username)
            .Select(a => a.UserName)
            .FirstOrDefaultAsync();

        if (admin != null)
        {
            return Ok(new { IsLoggedIn = true, AdminName = admin });
        }

        return Unauthorized(new { IsLoggedIn = false });
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