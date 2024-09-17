using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Services;

namespace StarterKit.Controllers;


[Route("api/v1/Login")]
public class LoginController : Controller
{
    private readonly ILoginService _loginService;


    public LoginController(ILoginService loginService)
    {
        _loginService = loginService;
    }






    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginBody loginBody)
    {
        var loginStatus = _loginService.CheckPassword(loginBody.Username, loginBody.Password);

        switch (loginStatus)
        {
            case LoginStatus.Success:
                return Ok("Login successful");

            case LoginStatus.IncorrectUsername:
                return Unauthorized("Incorrect username");

            case LoginStatus.IncorrectPassword:
            default:
                return Unauthorized("Incorrect password");
        }
    }

    [HttpGet("IsAdminLoggedIn")]
    public IActionResult IsAdminLoggedIn()
    {
        // TODO: This method should return a status 200 OK when logged in, else 403, unauthorized
        return Unauthorized("You are not logged in");
    }

    [HttpGet("Logout")]
    public IActionResult Logout()
    {
        return Ok("Logged out");
    }

}

public class LoginBody
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}