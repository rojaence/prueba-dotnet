using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Contexts;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace backend.Controllers;

[Route("api/session")]
[ApiController]
public class LoginController(ConnSqlServer context, IConfiguration configuration) : ControllerBase
{
  private readonly ConnSqlServer _context = context;
  private readonly IConfiguration _configuration = configuration;

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUser) 
  {
    var userExist = await _context.Users.AnyAsync(u => u.Username == loginUser.Username);

    if (!userExist) return BadRequest("Usuario no existe");
    var user = await _context.Users.FirstAsync(u => u.Username == loginUser.Username);
    var userRoleId = await _context.RoleUsers
              .Where(ru => ru.IdUser == user.IdUser)
              .Select(ru => ru.IdRole)
              .FirstOrDefaultAsync();
    var role = await _context.Roles
              .Where(r => r.IdRole == userRoleId)
              .Select(r => r.RoleName)
              .FirstOrDefaultAsync();
    
    if (user.SessionActive) {
      var lastSession = await _context.Sessions
          .Where(s => s.IdUser == user.IdUser && s.EndDate == null)
          .OrderByDescending(s => s.StartDate)
          .FirstOrDefaultAsync();
      if (lastSession != null) lastSession.EndDate = DateTime.Now;
      // return Ok(new { message = true });
    }
    var session = new Session 
    {
      StartDate = DateTime.Now,
      IdUser = user.IdUser
    };

    user.SessionActive = true;

    _context.Entry(user).State = EntityState.Modified;
    _context.Sessions.Add(session);
    await _context.SaveChangesAsync();    
    
    if (!BCrypt.Net.BCrypt.EnhancedVerify(loginUser.Password, user.Password)) return BadRequest("Credenciales incorrectas");
    var token = GenerateJwtToken(user.Username!, role!, session.StartDate, user.IdUser);
    SetTokenCookie(token);
    return Ok(new { success = true });
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout()
  {
    var token = Request.Cookies["authToken"];

    if (string.IsNullOrEmpty(token))
    {
        return Unauthorized(new { message = "No hay autorizacion" });
    }

    var claimsPrincipal = ValidateJwtToken(token);

    if (claimsPrincipal == null)
    {
        return Unauthorized(new { message = "Autorizacion invalida" });
    }

    var name = claimsPrincipal.FindFirst("name")?.Value;

    var userExist = await _context.Users.AnyAsync(u => u.Username == name);
    if (!userExist) return BadRequest("Usuario no existe");
    if (Request.Cookies["authToken"] != null)
    {
        // Establecer una cookie con el mismo nombre pero con expiración inmediata
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(-1),  // Expirar inmediatamente
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };

        Response.Cookies.Append("authToken", "", cookieOptions);
    }
    var user = await _context.Users.FirstAsync(u => u.Username == name);
    var lastSession = await _context.Sessions
          .Where(s => s.IdUser == user.IdUser && s.EndDate == null)
          .OrderByDescending(s => s.StartDate)
          .FirstOrDefaultAsync();
    lastSession!.EndDate = DateTime.Now;
    user.SessionActive = false;
    await _context.SaveChangesAsync();
    return Ok(new { success = true });
  }

  [HttpGet("check-auth")]
  [Authorize]
  public IActionResult CheckAuth() 
  {
    return Ok(new { authenticated = true });
  }

  [HttpGet("user-data")]
  [Authorize]
  public async Task<ActionResult<UserDTO>> GetUserData()
  {
    var userId = User.FindFirstValue("id");
    var date = User.FindFirstValue("date");
    var username = User.FindFirstValue("name");
    if (userId == null) return Unauthorized();
    var user = await _context.Users.FirstAsync(u => u.Username == username);
    if (user == null) return NotFound();
    var userRoleId = await _context.RoleUsers
              .Where(ru => ru.IdUser == user.IdUser)
              .Select(ru => ru.IdRole)
              .FirstOrDefaultAsync();
    var role = await _context.Roles
              .Where(r => r.IdRole == userRoleId)
              .Select(r => r.RoleName)
              .FirstOrDefaultAsync();
    var lastSession = await _context.Sessions
          .Where(s => s.IdUser == user.IdUser && s.EndDate == null)
          .OrderByDescending(s => s.StartDate)
          .FirstOrDefaultAsync();
    return Ok(new {
      user.IdUser,
      user.Username,
      user.SessionActive,
      user.Email,
      user.Status,
      user.FirstName,
      user.MiddleName,
      user.FirstLastname,
      user.SecondLastname,
      user.IdCard,
      user.BirthDate,
      Role = role,
      LastSession = lastSession!.StartDate
    });
  }

  private string GenerateJwtToken(string username, string role, DateTime date, int id)
  {
      var claims = new List<Claim>
    {
        new Claim("name", username),
        new Claim("role", role),
        new Claim("date", date.ToString()),
        new Claim("id", id.ToString()) 
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddMinutes(30),
        signingCredentials: creds
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    return tokenString;
  }

    private void SetTokenCookie(string token)
    {
        // Configurar la cookie HttpOnly, Secure y SameSite
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(30)
        };
        Response.Cookies.Append("authToken", token, cookieOptions);
    }
   private ClaimsPrincipal ValidateJwtToken(string token)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

    try
    {
        // Configurar los parámetros de validación de tokens según la configuración actual
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

        // Verificar si el token es un JWT válido
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