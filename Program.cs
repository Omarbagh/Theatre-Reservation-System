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
            builder.Services.AddScoped<AdminAuthFilter>();

            builder.Services.AddDbContext<DatabaseContext>(
                options => options.UseSqlite(builder.Configuration.GetConnectionString("SqlLiteDb")));

            var app = builder.Build();
            builder.Services.AddLogging();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<JsonContentMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();


            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();

        }
    }
}