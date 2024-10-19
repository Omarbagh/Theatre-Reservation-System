using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace StarterKit.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public List<Reservation>? Reservations { get; set; }
    }

    public class Reservation
    {
        public int ReservationId { get; set; }
        public int AmountOfTickets { get; set; }
        public bool Used { get; set; }

        public int CustomerId { get; set; }
        public int TheatreShowDateId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual TheatreShowDate TheatreShowDate { get; set; }
    }


    public class TheatreShowDate
    {
        public int TheatreShowDateId { get; set; }

        public DateTime DateAndTime { get; set; } //"MM-dd-yyyy HH:mm"

        public List<Reservation>? Reservations { get; set; }

        public TheatreShow? TheatreShow { get; set; }

    }

    public class TheatreShow
    {
        public int TheatreShowId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public double Price { get; set; }

        public List<TheatreShowDate>? theatreShowDates { get; set; }

        public Venue? Venue { get; set; }

    }

    public class Venue
    {
        public int VenueId { get; set; }

        public string? Name { get; set; }

        public int Capacity { get; set; }

        public List<TheatreShow>? TheatreShows { get; set; }
    }

    public class Snacks
    {
        public int SnacksId { get; set; }
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public int Amount { get; set; }

        public int Price { get; set; }
    }

    public class ReservationSnack
    {
        public int ReservationSnackId { get; set; } // Primary Key
        public int ReservationId { get; set; } // Foreign Key to Reservation
        public Reservation Reservation { get; set; } // Navigation property to Reservation

        public int SnacksId { get; set; } // Foreign Key to Snacks
        public Snacks Snack { get; set; } // Navigation property to Snacks

        public int Amount { get; set; } // Amount ordered
        public int TotalPrice { get; set; } // Total price for all snacks (Amount * Snack Price)
    }


    public class AdminDashboard
    {
        [Key]
        public int ReservationId { get; set; }
        public int CustomerId { get; set; }
        public int TheatreShowId { get; set; }
        public int VenueId { get; set; }
        public int AmountOfTickets { get; set; }
        public decimal TotalPrice { get; set; }
        public string? SnacksDetails { get; set; }
        public DateTime DateAndTime { get; set; }
        public bool ReservationUsed { get; set; }

    }



}