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
        // Get the username from the session
        var username = _httpContextAccessor.HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            // No username in session
            context.Result = new UnauthorizedObjectResult(new { IsLoggedIn = false });
            return;
        }

        // Check if the username exists in the Admin table
        var admin = await _context.Admin
            .Where(a => a.UserName == username)
            .FirstOrDefaultAsync();

        if (admin == null)
        {
            // If the admin is not found, return Unauthorized
            context.Result = new UnauthorizedObjectResult(new { IsLoggedIn = false });
            return;
        }

        // Admin found, continue to the action
        await next();
    }
}
