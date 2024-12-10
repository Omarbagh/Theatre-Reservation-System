using Microsoft.EntityFrameworkCore;
using Services;
using StarterKit.Models;
using StarterKit.Services;

namespace StarterKit
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.AddConsole();

            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(200);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<ShowService>();
            builder.Services.AddScoped<SnackService>();
            builder.Services.AddScoped<SnackReservationService>();
            builder.Services.AddScoped<ReservationManagementService>();
            builder.Services.AddScoped<AdminAuthFilter>();
            builder.Services.AddScoped<IReservationService, ReservationService>();
            builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();

            builder.Services.AddDbContext<DatabaseContext>(
                options => options.UseSqlite(builder.Configuration.GetConnectionString("SqlLiteDb")));

            // Add CORS policy to allow frontend on port 8080
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:8080")  // Allow your frontend's URL
                          .AllowAnyMethod()  // Allow any HTTP method (GET, POST, etc.)
                          .AllowAnyHeader()  // Allow any headers
                          .AllowCredentials(); // Allow cookies for session management
                });
            });

            var app = builder.Build();
            builder.Services.AddLogging();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<JsonContentMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();

            // Enable CORS globally
            app.UseCors("AllowFrontend");

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
