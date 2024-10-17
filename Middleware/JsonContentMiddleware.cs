public class JsonContentMiddleware
{
    private readonly RequestDelegate _next;

    public JsonContentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Uitsluiten van bepaalde routes zoals de root '/' of '/Home/Error'
        var path = context.Request.Path.Value;

        if (path == "/" || path.StartsWith("/Home"))
        {
            // Ga verder zonder foutmelding voor root en specifieke routes
            await _next(context);
            return;
        }

        // Controleer of het pad begint met 'api/v1/'
        if (!context.Request.Path.StartsWithSegments("/api/v1"))
        {
            // Stel de statuscode en de foutboodschap in
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                Message = "Ongeldig verzoek: het pad moet beginnen met '/api/v1/'."
            };

            // Schrijf de foutmelding als JSON terug naar de response
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
        else
        {
            // Ga verder met de volgende middleware als het pad correct is
            await _next(context);
        }
    }
}
