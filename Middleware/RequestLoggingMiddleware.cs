using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Maak een logbericht aan
        var logMessage = $"{DateTime.Now}: {context.Request.Method} {context.Request.Path} from {context.Connection.RemoteIpAddress}\n";

        // Definieer het bestandspad voor de logs
        var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "requests.log");

        // Zorg ervoor dat de logdirectory bestaat
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

        // Log naar het bestand
        await File.AppendAllTextAsync(logFilePath, logMessage);

        // Ga verder met de volgende middleware in de pijplijn
        await _next(context);
    }
}
