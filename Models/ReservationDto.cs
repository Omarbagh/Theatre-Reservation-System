public class ReservationDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int ShowDateId { get; set; }
    public int AmountOfTickets { get; set; }
}

public class ReservationResponseDto
{
    public int ReservationId { get; set; }
    public int AmountOfTickets { get; set; }
    public bool Used { get; set; }
    public CustomerDto Customer { get; set; }
    public TheatreShowDateDto TheatreShowDate { get; set; }
}

public class TheatreShowDateDto
{
    public int TheatreShowDateId { get; set; }
    public DateTime DateAndTime { get; set; }
}

public class CustomerDto
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}