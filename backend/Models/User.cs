using System.ComponentModel.DataAnnotations;
using backend.Enums;

namespace backend.Models;

public class User {
  public int IdUser { get; set; }
  public string? Username { get; set; }
  public string? Password { get; set; }
  public bool SessionActive { get; set; }
  public string? Email { get; set; }
  public bool Status { get; set; }
  public string? FirstName { get; set; }
  public string? MiddleName { get; set; }
  public string? FirstLastname { get; set; }
  public string? SecondLastname { get; set; }
  public string? IdCard { get; set; }
  public DateTime BirthDate { get; set; }

  public ICollection<RoleUser> RoleUsers { get; set; }
  public ICollection<Session> Sessions { get; set; }
}

public class UserListItemDTO 
{
  public int IdUser { get; set; }
  public string? Username { get; set; }
  public bool SessionActive { get; set; }
  public string? Email { get; set; }
  public bool Status { get; set; }
  public string? FirstName { get; set; }
  public string? MiddleName { get; set; }
  public string? FirstLastname { get; set; }
  public string? SecondLastname { get; set; }
  public string? IdCard { get; set; } 
  public int IdSession { get; set; }
  public DateTime? StartDate { get; set; }

  public string? RoleName { get; set; }
}

public class UserDTO 
{
  public int IdUser { get; set; }
  public string? Username { get; set; }
  public bool SessionActive { get; set; }
  public string? Email { get; set; }
  public bool Status { get; set; }
  public string? FirstName { get; set; }
  public string? MiddleName { get; set; }
  public string? FirstLastname { get; set; }
  public string? SecondLastname { get; set; }
  public string? IdCard { get; set; }
  public DateTime BirthDate { get; set; }
}

public class CreateUserDTO 
{
  [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
  [StringLength(20, MinimumLength = 8, ErrorMessage = "El nombre de usuario debe tener entre 8 y 20 caracteres.")]
  [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$", ErrorMessage = "El nombre de usuario debe contener al menos una letra mayúscula y un número, y no puede contener signos.")]
  public string? Username { get; set; }
  
  [Required(ErrorMessage = "La contraseña es obligatoria.")]
  [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
  [RegularExpression(@"^(?=.*[A-Z])(?=.*\W)(?!.*\s).+$", ErrorMessage = "La contraseña debe contener al menos una letra mayúscula, un signo y no debe contener espacios.")]
  public string? Password { get; set; }

  [Required(ErrorMessage = "El primer nombre es obligatorio")]
  public string? FirstName { get; set; }

  [Required(ErrorMessage = "El segundo nombre es obligatorio.")]
  public string? MiddleName { get; set; }

  [Required(ErrorMessage = "El primer apellido es obligatorio.")]
  public string? FirstLastname { get; set; }

  [Required(ErrorMessage = "El segundo apellido es obligatorio.")]
  public string? SecondLastname { get; set; }

  [Required(ErrorMessage = "La identificacion es obligatoria.")]
  [StringLength(10, MinimumLength = 10, ErrorMessage = "La identificacion debe tener exactamente 10 dígitos.")]
  [RegularExpression(@"^\d+$", ErrorMessage = "La identifiacion debe contener solo números.")]
  [UserIdCardValidator(ErrorMessage = "La identificacion no debe tener el mismo numero 4 veces seguidas")]
  public string? IdCard { get; set; }
  [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
  public DateTime BirthDate { get; set; }
  [Required(ErrorMessage = "Se requiere especificar un rol para el usuario.")]
  public UserRoleEnum Role { get; set; }
}

public class LoginUserDTO 
{
  [Required(ErrorMessage = "La contraseña es obligatoria.")]
  public string? Password { get; set; }
  [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
  public string? Username { get; set; }
}

public class LogoutUserDTO
{
  [Required(ErrorMessage = "El nombre de usuario es requerido")]
  public string? Username { get; set; }
}

public class UpdateUserDTO
{
  [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
  [StringLength(20, MinimumLength = 8, ErrorMessage = "El nombre de usuario debe tener entre 8 y 20 caracteres.")]
  [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$", ErrorMessage = "El nombre de usuario debe contener al menos una letra mayúscula y un número, y no puede contener signos.")]
  public string? Username { get; set; }

  [Required(ErrorMessage = "El primer nombre es obligatorio")]
  public string? FirstName { get; set; }

  [Required(ErrorMessage = "El segundo nombre es obligatorio.")]
  public string? MiddleName { get; set; }

  [Required(ErrorMessage = "El primer apellido es obligatorio.")]
  public string? FirstLastname { get; set; }

  [Required(ErrorMessage = "El segundo apellido es obligatorio.")]
  public string? SecondLastname { get; set; }

  [Required(ErrorMessage = "La identificacion es obligatoria.")]
  [StringLength(10, MinimumLength = 10, ErrorMessage = "La identificacion debe tener exactamente 10 dígitos.")]
  [RegularExpression(@"^\d+$", ErrorMessage = "La identifiacion debe contener solo números.")]
  [UserIdCardValidator(ErrorMessage = "La identificacion no debe tener el mismo numero 4 veces seguidas")]
  public string? IdCard { get; set; }
  [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
  public DateTime BirthDate { get; set; }
}

public class UpdatePasswordDTO 
{
[Required(ErrorMessage = "La contraseña es obligatoria.")]
[StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
[RegularExpression(@"^(?=.*[A-Z])(?=.*\W)(?!.*\s).+$", ErrorMessage = "La contraseña debe contener al menos una letra mayúscula, un signo y no debe contener espacios.")]
public string? NewPassword { get; set; }
[Required(ErrorMessage = "La contraseña es obligatoria.")]
public string? CurrentPassword { get; set; }
[Required(ErrorMessage = "Repetir contraseña es obligatorio.")]
public string? RepeatPassword { get; set; }
}