using backend.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Authorize]
[Microsoft.AspNetCore.Mvc.Route("api/roles")]
[ApiController]
public class RolesController : ControllerBase
{
  private readonly ConnSqlServer _context;
  public RolesController(ConnSqlServer context)
  {
    _context = context;
  }

  [HttpGet]
  public async Task<IActionResult> GetRoles()
  {
    var roles = await _context.Roles
    .Select(r => new 
    {
      IdRole = r.IdRole,
      RoleName = r.RoleName
    })
    .ToListAsync();
    return Ok(roles);
  }
}