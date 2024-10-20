// Importeert benodigde namespaces voor JSON, controllers, modellen, utils en SQLite ondersteuning.
using System.Text.Json;
using StarterKit.Controllers;
using StarterKit.Models;
using StarterKit.Utils;
using System;
using Microsoft.Data.Sqlite;  // Het correcte pakket voor SQLite ondersteuning.
using System.Data.SqlTypes;

namespace StarterKit.Services;

// Definieert een enumeratie voor de verschillende mogelijke inlogstatussen.
public enum LoginStatus { IncorrectPassword, IncorrectUsername, Success }

// Definieert een enumeratie voor de sessiesleutel die aangeeft of een admin is ingelogd.
public enum ADMIN_SESSION_KEY { adminLoggedIn }

// Definieert een serviceklasse die verantwoordelijk is voor het inloggen.
public class LoginService : ILoginService
{
    // Definieert een privé variabele die de databasecontext opslaat.
    // Hiermee wordt toegang verkregen tot de database.
    private readonly DatabaseContext _context;

    // Constructor van de LoginService klasse.
    // De constructor neemt een DatabaseContext als parameter en wijst deze toe aan de privé variabele.
    public LoginService(DatabaseContext context)
    {
        _context = context;
    }

    // Methode die controleert of de ingevoerde wachtwoord correct is voor de opgegeven gebruikersnaam.
    public LoginStatus CheckPassword(string username, string inputPassword)
    {
        // Geeft het pad naar je SQLite databasebestand op.
        string connectionString = @"Data Source=webdev.sqlite";

        // SQL query om het gehashte wachtwoord op te halen voor de gegeven gebruikersnaam.
        string query = "SELECT Password FROM Admin WHERE UserName = @username";

        try
        {
            // Maakt een verbinding met de SQLite database.
            using (var conn = new SqliteConnection(connectionString))
            {
                // Opent de databaseverbinding.
                conn.Open();

                // Creëert een SQLite command met de opgegeven query en verbinding.
                using (var cmd = new SqliteCommand(query, conn))
                {
                    // Gebruikt een geparametriseerde query om SQL-injectie te voorkomen.
                    cmd.Parameters.AddWithValue("@username", username);

                    // Voert de query uit en haalt het resultaat op (het gehashte wachtwoord).
                    object result = cmd.ExecuteScalar();

                    // Als een resultaat wordt gevonden, wordt het ingevoerde wachtwoord vergeleken met het opgeslagen wachtwoord.
                    if (result != null)
                    {
                        // Zet het resultaat om naar een string (het opgeslagen gehashte wachtwoord).
                        string storedPassword = result.ToString();
                        // Hash het ingevoerde wachtwoord.
                        string hashedInputPassword = EncryptionHelper.EncryptPassword(inputPassword);
                        // Vergelijkt de gehashte wachtwoorden.

                        // Als de wachtwoorden overeenkomen, retourneer success, anders incorrect wachtwoord.
                        return hashedInputPassword == storedPassword ? LoginStatus.Success : LoginStatus.IncorrectPassword;
                    }
                    else
                    {
                        // Retourneer incorrecte gebruikersnaam als er geen resultaat is gevonden.
                        return LoginStatus.IncorrectUsername;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Print een foutmelding als er een uitzondering optreedt en retourneer incorrecte gebruikersnaam.
            Console.WriteLine("An error occurred: " + ex.Message);
            return LoginStatus.IncorrectUsername;
        }
    }

}
