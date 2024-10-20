// Importeert de benodigde namespaces voor ASP.NET Core MVC en asynchrone taken.
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

// Definieert de route voor de API-controller. Deze controller handelt reserveringsverzoeken af.
[Route("api/v1/Reservation")]
[ApiController]
public class ReservationController : ControllerBase
{
    // Definieert een privé variabele die de reserveringsservice opslaat.
    // Deze service wordt gebruikt om reserveringen te maken.
    private readonly IReservationService _reservationService;

    // Constructor van de ReservationController klasse.
    // De constructor neemt een IReservationService als parameter en wijst deze toe aan de privé variabele.
    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }


    //     {
    //     "reservation": {
    //         "ShowDateId": 1,
    //         "FirstName": "John",
    //         "LastName": "Doe",
    //         "Email": "john.doe@example.com",
    //         "AmountOfTickets": 3
    //     },
    //     "snackOrders": [
    //         {
    //             "id": 1,
    //             "amount": 2
    //         },
    //         {
    //             "id": 3,
    //             "amount": 1
    //         }
    //     ]
    // }  DIT INVULLEN BIJ JSON VOOR AANMAKEN RESERVATION


    // HTTP POST methode voor het creëren van een nieuwe reservering.
    // De methode accepteert een ReservationRequestDto als JSON in de body van de aanvraag.
    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationRequestDto requestDto)
    {
        // Controleert of het model geldig is; retourneert BadRequest als dat niet het geval is.
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Roept de CreateReservationAsync methode van de reserveringsservice aan
            // en geeft zowel de reservering als de snackbestellingen door.
            var result = await _reservationService.CreateReservationAsync(requestDto.Reservation, requestDto.SnackOrders);
            // Retourneert een CreatedAtAction met de details van de nieuwe reservering.
            return CreatedAtAction(nameof(CreateReservation), new { id = result.ReservationId }, result);
        }
        catch (KeyNotFoundException ex)
        {
            // Retourneert een NotFound status als een gevraagde resource niet is gevonden.
            return NotFound(ex.Message);
        }
        catch
        {
            // Retourneert een interne serverfoutstatus als er een andere fout optreedt.
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
