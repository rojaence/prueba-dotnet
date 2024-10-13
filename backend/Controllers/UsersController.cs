using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Contexts;
using backend.Models;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.IO.Compression;

namespace backend.Controllers;

[Authorize]
[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
  private readonly ConnSqlServer _context;
  private readonly IMapper _mapper;

  public UsersController(ConnSqlServer context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  [HttpGet]
  public async Task<IActionResult> GetUsers()
  {
    var users = await _context.GetUsersWithDetailsAsync();
    return Ok(users);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<UserDTO>> GetUser(int id)
  {
    var user = await _context.Users.FirstOrDefaultAsync(a => a.IdUser == id);
    if (user == null) return NotFound();
    var userDTO = _mapper.Map<UserDTO>(user);
    return userDTO;
  }

  [HttpPost]
  public async Task<ActionResult<User>> PostUser([FromBody] CreateUserDTO newUser)
  {
    // VALIDAR DATOS
    if (!ModelState.IsValid) return BadRequest(ModelState);

    // COMPROBAR USERNAME
    var isDuplicated = await IsUsernameDuplicated(newUser.Username!);
    if (isDuplicated) return BadRequest(new { errors = "El nombre de usuario ya existe"});

    var isDuplicatedIdCard = await IsIdCardDuplicated(newUser.IdCard!);
    if (isDuplicated) return BadRequest(new { errors = "La identificación de usuario ya existe"});

    // GENERAR EMAIL ÚNICO
    var email = await GenerateEmail(newUser);


    var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(newUser.Password, 13);
    var user = new User
    {
      Username = newUser.Username,
      Password = passwordHash,
      Email = email,
      FirstName = newUser.FirstName,
      MiddleName = newUser.MiddleName,
      FirstLastname = newUser.FirstLastname,
      SecondLastname = newUser.SecondLastname,
      IdCard = newUser.IdCard,
      BirthDate = newUser.BirthDate
    };

    // Agregar usuario si todo marcha bien
    _context.Users.Add(user);

    var idRole = 2;

    if (newUser.Role == Enums.UserRoleEnum.Admin) idRole = 1;

    // GUARDAR LOS PERMISOS DE ROL
    var userRol = new RoleUser
    {
      IdRole = idRole,
      IdUser = user.IdUser
    };
    
    return CreatedAtAction(nameof(GetUser), new { id = user.IdUser }, user);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateUserDTO userData)
  {
    if (!ModelState.IsValid) return BadRequest(ModelState);
    // COMPROBAR USERNAME
    var user = await _context.Users.FirstOrDefaultAsync(u => u.IdUser == id);
    if (user == null) return BadRequest(new { errors = "El usuario no existe"});
    if (userData.Username != user.Username)  {
      var isDuplicated = await IsUsernameDuplicated(userData.Username!);
      if (isDuplicated) return BadRequest(new { errors = "El nombre de usuario ya existe"});
    }
    if (userData.IdCard != user.IdCard) {
      var isDuplicatedIdCard = await IsIdCardDuplicated(userData.IdCard!);
      if (isDuplicatedIdCard) return BadRequest(new { errors = "La identificación de usuario ya existe"});
    }
    user.FirstName = userData.FirstName;
    user.MiddleName = userData.MiddleName;
    user.FirstLastname = userData.FirstLastname;
    user.SecondLastname = userData.SecondLastname;
    user.IdCard = userData.IdCard;
    user.Username = userData.Username;
    user.BirthDate = userData.BirthDate;
    await _context.SaveChangesAsync();
    return Ok(new { success = true });
  }

  [HttpGet("/{id}/permissions")]
  public async Task<ActionResult<IEnumerable<PermissionDTO>>> GetPermissions(int id)
  {
    var permissions = await _context.Users
    .Where(u => u.IdUser == id)
    .SelectMany(u => u.RoleUsers)
    .SelectMany(ru => ru.Role.RolePermissions)
    .Select(rp => rp.Permission)
    .Distinct()
    .ToListAsync();
    
    // return permissions;
    return _mapper.Map<List<PermissionDTO>>(permissions);
  }

  [HttpPost("{id}/password")]
  public async Task<IActionResult> UpdatePassword(int id, [FromBody] UpdatePasswordDTO userData)
  {
    if (!ModelState.IsValid) return BadRequest(ModelState);
    var user = await _context.Users.FirstAsync(u => u.IdUser == id);
    if (user == null) return BadRequest("Usuario no existe");
    if (!BCrypt.Net.BCrypt.EnhancedVerify(userData.CurrentPassword, user.Password)) return BadRequest("Contraseña actual incorrecta");
    if (userData.NewPassword != userData.RepeatPassword) return BadRequest("Campos de contraseña nueva no coinciden");

    // Generar nuevo hash de password y guardar
    var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(userData.NewPassword, 13);
    user.Password = passwordHash;
    await _context.SaveChangesAsync();
    return Ok(new { success = true });
  }


  [ApiExplorerSettings(IgnoreApi = true)]
  public async Task<string> GenerateEmail(CreateUserDTO newUser) 
  {
    var first = newUser.FirstName![0].ToString().ToLower();
    var middle = newUser.FirstLastname!.ToLower();
    var end = newUser.SecondLastname![0].ToString().ToLower();

    var email = "{first}{middle}{end}@mail.com";
    int counter = 1;

    var emailParts = email.Split('@');
    var usernamePart = emailParts[0];
    var domainPart = emailParts[1];

    // Verificar si el email ya existe en la base de datos
    while (await _context.Users.AnyAsync(u => u.Email == email))
    {
        // Generar un nuevo email con el contador
        email = $"{usernamePart}{counter}@{domainPart}";
        counter++;
    }
    return email;
  }

  [ApiExplorerSettings(IgnoreApi = true)]
   public async Task<bool> IsUsernameDuplicated(string username)
    {
        var isDuplicated = await _context.Users.AnyAsync(u => u.Username == username);
        return isDuplicated;
    }

  protected async Task<bool> IsIdCardDuplicated(string idCard)
  {
      var isDuplicated = await _context.Users.AnyAsync(u => u.IdCard == idCard);
      return isDuplicated;
  }
}

