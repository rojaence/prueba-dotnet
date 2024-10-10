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
      return Ok();
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
    return Ok(new { token });
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout(LogoutUserDTO userDTO)
  {
    var userExist = await _context.Users.AnyAsync(u => u.Username == userDTO.Username);
    Console.WriteLine(userDTO.Username);
    if (!userExist) return BadRequest("Usuario no existe");
    var user = await _context.Users.FirstAsync(u => u.Username == userDTO.Username);
    var lastSession = await _context.Sessions
          .Where(s => s.IdUser == user.IdUser && s.EndDate == null)
          .OrderByDescending(s => s.StartDate)
          .FirstOrDefaultAsync();
    lastSession!.EndDate = DateTime.Now;
    user.SessionActive = false;
    await _context.SaveChangesAsync();
    return Ok(new { success = true });
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
}