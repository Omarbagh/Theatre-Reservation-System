using System.Text.Json;
using StarterKit.Controllers;
using StarterKit.Models;
using StarterKit.Utils;
using System;
using Microsoft.Data.Sqlite;  // Correct package for SQLite support
using System.Data.SqlTypes;

namespace StarterKit.Services;

public enum LoginStatus { IncorrectPassword, IncorrectUsername, Success }

public enum ADMIN_SESSION_KEY { adminLoggedIn }

public class LoginService : ILoginService
{

    private readonly DatabaseContext _context;

    public LoginService(DatabaseContext context)
    {
        _context = context;
    }

    public LoginStatus CheckPassword(string username, string inputPassword)
    {
        // Path to your SQLite database file
        string connectionString = @"Data Source=webdev.sqlite";

        // SQL query to fetch the hashed password for the given username
        string query = "SELECT Password FROM Admin WHERE UserName = @username";

        try
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new SqliteCommand(query, conn))
                {
                    // Use parameterized query to avoid SQL injection
                    cmd.Parameters.AddWithValue("@username", username);

                    // Execute the query and fetch the result
                    object result = cmd.ExecuteScalar();

                    // If a result is found, compare the input password with the stored password
                    if (result != null)
                    {
                        string storedPassword = result.ToString();
                        // Hash the input password
                        string hashedInputPassword = EncryptionHelper.EncryptPassword(inputPassword);
                        // Compare hashed passwords

                        return hashedInputPassword == storedPassword ? LoginStatus.Success : LoginStatus.IncorrectPassword;
                    }
                    else
                    {
                        return LoginStatus.IncorrectUsername;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return LoginStatus.IncorrectUsername;
        }
    }

}