using Microsoft.EntityFrameworkCore;
using StarterKit.Utils;

using Microsoft.EntityFrameworkCore;

namespace StarterKit.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<TheatreShowDate> TheatreShowDate { get; set; }
        public DbSet<TheatreShow> TheatreShow { get; set; }
        public DbSet<Venue> Venue { get; set; }

        public DbSet<ReservationSnack> ReservationSnacks { get; set; }

        public DbSet<Snacks> Snacks { get; set; }

        public DbSet<AdminDashboard> AdminDashboards { get; set; }


        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>()
                .HasIndex(p => p.UserName).IsUnique();

            modelBuilder.Entity<Admin>()
                .HasData(new Admin { AdminId = 1, Email = "admin1@example.com", UserName = "admin1", Password = EncryptionHelper.EncryptPassword("password") });
            modelBuilder.Entity<Admin>()
                .HasData(new Admin { AdminId = 2, Email = "admin2@example.com", UserName = "admin2", Password = EncryptionHelper.EncryptPassword("tooeasytooguess") });
            modelBuilder.Entity<Admin>()
                .HasData(new Admin { AdminId = 3, Email = "admin3@example.com", UserName = "admin3", Password = EncryptionHelper.EncryptPassword("helloworld") });
            modelBuilder.Entity<Admin>()
                .HasData(new Admin { AdminId = 4, Email = "admin4@example.com", UserName = "admin4", Password = EncryptionHelper.EncryptPassword("Welcome123") });
            modelBuilder.Entity<Admin>()
                .HasData(new Admin { AdminId = 5, Email = "admin5@example.com", UserName = "admin5", Password = EncryptionHelper.EncryptPassword("Whatisapassword?") });
        }
    }
}
