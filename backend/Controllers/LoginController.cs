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
public class LoginController(ConnSqlServer context, IConfiguration configuration, IWebHostEnvironment env) : ControllerBase
{
  private readonly ConnSqlServer _context = context;
  private readonly IConfiguration _configuration = configuration;
    private readonly IWebHostEnvironment _env = env;

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUser) 
  {
    var userExist = await _context.Users.AnyAsync(u => u.Username == loginUser.Username);
    if (!userExist) return BadRequest("Usuario no existe");
    var user = await _context.Users.FirstAsync(u => u.Username == loginUser.Username);

    if (!user.Status) return BadRequest("El usuario se encuentra deshabilitado, comuniquese con un administrador");

    // Verificar intentos de inicio de sesión fallidos
    var logginAttempts = await GetLoginAttemps(user);
  /*  if (logginAttempts >= 3)
        {
            return BadRequest("Intentos de inicio de sesión superados, comuniquese con un administrador");
        }*/

        // Verificar credenciales
        if (!BCrypt.Net.BCrypt.EnhancedVerify(loginUser.Password, user.Password))
        {
            var loginAttempt = new LoginAttempt
            {
                IdUser = user.IdUser,
            };
            var attempts = await GetLoginAttemps(user);
            if (attempts >= 3)
            {
                user.SessionActive = false;
                user.Status = false;
                var lastSession = await GetLastSession(user.IdUser);
                if (lastSession != null) lastSession.EndDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return BadRequest("Intentos de inicio de sesión superados, comuniquese con un administrador");
            }
            _context.LoginAttempts.Add(loginAttempt);
            await _context.SaveChangesAsync();
            return BadRequest("Credenciales incorrectas");
        };

        await RevokeAttempts(user.IdUser);

        var userRoleId = await _context.RoleUsers
              .Where(ru => ru.IdUser == user.IdUser)
              .Select(ru => ru.IdRole)
              .FirstOrDefaultAsync();
    var role = await _context.Roles
              .Where(r => r.IdRole == userRoleId)
              .Select(r => r.RoleName)
              .FirstOrDefaultAsync();
    
    if (user.SessionActive) {
            var lastSession = await GetLastSession(user.IdUser);
            if (lastSession != null) lastSession.EndDate = DateTime.Now;
    }


    var session = new Session 
    {
      StartDate = DateTime.Now,
      IdUser = user.IdUser
    };

    user.SessionActive = true;

    var token = GenerateJwtToken(user.Username!, role!, session.StartDate, user.IdUser);
    SetTokenCookie(token);

    _context.Entry(user).State = EntityState.Modified;
    _context.Sessions.Add(session);
    await _context.SaveChangesAsync();    
    
    return Ok(new { success = true });
  }

    private async Task<int> GetLoginAttemps(User user)
    {
        var loginAttempts = await _context.LoginAttempts
            .Where(la => la.IdUser == user.IdUser && !la.Resolved)
            .OrderByDescending(la => la.DateAttempt)
            .ToListAsync();
        return loginAttempts.Count;
    }

    private async Task<Session?> GetLastSession(int idUser)
    {
        var lastSession = await _context.Sessions
          .Where(s => s.IdUser == idUser && s.EndDate == null)
          .OrderByDescending(s => s.StartDate)
          .FirstOrDefaultAsync();
        return lastSession;
    }

    [HttpPost("logout")]
  public async Task<IActionResult> Logout()
  {
    var token = Request.Cookies["authToken"];

    if (string.IsNullOrEmpty(token))
    {
        return Unauthorized(new { message = "No hay autorizacion" });
    }

    var claimsPrincipal = await ValidateJwtToken(token);

    if (claimsPrincipal == null)
    {
        return Unauthorized(new { message = "Autorizacion invalida" });
    }

    var name = claimsPrincipal.FindFirst("name")?.Value;
    var id = claimsPrincipal.FindFirst("id")?.Value;

    var userExist = await _context.Users.AnyAsync(u => u.IdUser == int.Parse(id));
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
    var user = await _context.Users.FirstAsync(u => u.IdUser == int.Parse(id!));
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
  public async Task<IActionResult> CheckAuth() 
  {
        var token = Request.Cookies["authToken"];

        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new { autenthicated = false });
        }

        var claimsPrincipal = await ValidateJwtToken(token);

        if (claimsPrincipal == null)
        {
            // Responder con un mensaje de error si el token es inválido
            return Unauthorized(new { autenthicated = false });
        }

        var userId = User.FindFirstValue("id");
        if (userId == null) return Unauthorized(new { message = "Id no encontrado" });
        var user = await _context.Users.FirstAsync(u => u.IdUser == int.Parse(userId));
        var lastSession = await _context.Sessions
          .Where(s => s.IdUser == user.IdUser && s.EndDate == null)
          .OrderByDescending(s => s.StartDate)
          .FirstOrDefaultAsync();
        if (lastSession == null) return Unauthorized(new { autenthicated = false });
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
    var user = await _context.Users.FirstAsync(u => u.IdUser == int.Parse(userId));
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
    if (lastSession == null) return Unauthorized("Usuario no tiene sesión activa");
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
    [HttpPost("revoke-failed-login-attempts")]
    [Authorize]
    public async Task RevokeFailedLoginAttempts([FromBody] int idUser)
    {
        await RevokeAttempts(idUser);
    }

    [HttpPost("toggle-status")]
    [Authorize]
    public async Task<IActionResult> ToggleUserStatus([FromBody] UserStatusDTO statusDTO)
    {
        var user = await _context.Users.FirstAsync(u => u.IdUser == statusDTO.IdUser);
        if (user == null) return BadRequest("Usuario no existe");
        user.Status = statusDTO.Status;
        var lastSession = await GetLastSession(user.IdUser);
        if (lastSession != null) lastSession.EndDate = DateTime.Now;
        await _context.SaveChangesAsync();
        if (statusDTO.Status == true) return await RevokeAttempts(statusDTO.IdUser);
        return Ok(new { sucess = true });
    }

    private async Task<IActionResult> RevokeAttempts(int idUser)
    {
        var user = await _context.Users.FirstAsync(u => u.IdUser == idUser);
        if (user == null) return BadRequest("Usuario no existe");
        var failedAttempts = await _context.LoginAttempts.Where(l => l.IdUser == idUser && !l.Resolved).ToListAsync();
        foreach (var attempt in failedAttempts)
        {
            attempt.Resolved = true;
        }
        user.Status = true;
        await _context.SaveChangesAsync();
        return Ok(new { sucess = true });
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
            Secure = !_env.IsDevelopment(), // Solo asegurar en producción
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(30)
        };
        Response.Cookies.Append("authToken", token, cookieOptions);
    }
   private async Task<ClaimsPrincipal?> ValidateJwtToken(string token)
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
            var userId = principal.FindFirst("id")?.Value;

            if (userId != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.IdUser == int.Parse(userId));
                if (user != null && !user.Status) return null;
            }

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