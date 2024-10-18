using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Middlewares;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public TokenValidationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Obt�n el token del encabezado de autorizaci�n
        var token = context.Request.Cookies["authToken"];

        // Si no se proporciona el token, devuelve una respuesta 401
        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token de sesi�n es requerido.");
            return;
        }

        // Valida el token usando la funci�n proporcionada (interfaz o servicio)
        var isValidToken = ValidateJwtToken(token);

        if (isValidToken == null)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Sesi�n inv�lida");
            return;
        }

        // Si el token es v�lido, contin�a con la solicitud
        await _next(context);
    }

    private ClaimsPrincipal? ValidateJwtToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

        try
        {
            // Configurar los par�metros de validaci�n de tokens seg�n la configuraci�n actual
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // Intentar validar el token y devolver el principal (los claims)
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var userId = principal.FindFirst("id")?.Value;

            // Verificar si el token es un JWT v�lido
            if (validatedToken is JwtSecurityToken jwtToken)
            {
                return principal;
            }
            Console.WriteLine("Algo salio mal al validar el token");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return null;
        }
    }
}
