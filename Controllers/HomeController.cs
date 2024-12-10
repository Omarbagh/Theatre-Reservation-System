using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;

namespace StarterKit.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<LoginController> _logger;


    public HomeController(ILogger<LoginController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return Redirect("api/v1/login");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
