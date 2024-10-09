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
    
    if (!BCrypt.Net.BCrypt.EnhancedVerify(loginUser.Password, user.Password)) return BadRequest("Credenciales incorrectas");
    var token = GenerateJwtToken();
    return Ok(new { token });
  }


  /* public async Task<IActionResult> Logout(LogoutUserDTO user)
  {

  } */

  private string GenerateJwtToken()
  {
      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

      var claims = new[]
      {
          new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
      };

      var token = new JwtSecurityToken(
          issuer: _configuration["Jwt:Issuer"],
          audience: _configuration["Jwt:Audience"],
          claims: claims,
          expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
          signingCredentials: credentials
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
  }
}