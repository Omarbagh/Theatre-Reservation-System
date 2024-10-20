using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StarterKit.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class AdminAuthFilter : IAsyncActionFilter
{
    private readonly DatabaseContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminAuthFilter(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Haal de gebruikersnaam op uit de sessie
        var username = _httpContextAccessor.HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username)) // Controleert of er geen gebruikersnaam in de sessie is.
        {
            // Geen gebruikersnaam in sessie
            context.Result = new UnauthorizedObjectResult(new { IsLoggedIn = false }); // Retourneert een ongeautoriseerde respons.
            return; // Verlaat de methode.
        }

        // Controleer of de gebruikersnaam bestaat in de Admin-tabel
        var admin = await _context.Admin
            .Where(a => a.UserName == username) // Zoekt naar een admin met de opgegeven gebruikersnaam.
            .FirstOrDefaultAsync();

        if (admin == null) // Controleert of de admin niet is gevonden.
        {
            // Als de admin niet is gevonden, retourneer ongeautoriseerd
            context.Result = new UnauthorizedObjectResult(new { IsLoggedIn = false }); // Retourneert een ongeautoriseerde respons.
            return; // Verlaat de methode.
        }

        // Admin gevonden, ga verder met de actie
        await next(); // Voert de volgende actie in de pijplijn uit.
    }
}
