using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Contexts;
using backend.Models;
using AutoMapper;

namespace backend.Controllers;

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
  public async Task<ActionResult<IEnumerable<User>>> GetUsers()
  {
    return await _context.Users.ToListAsync();
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
    if (isDuplicated) return BadRequest("El nombre de usuario ya existe");

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
}

