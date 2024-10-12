namespace backend.Models;

public class Role
{
  public int IdRole { get; set; }
  public string? RoleName { get; set; }
  public ICollection<RolePermission> RolePermissions { get; set; } = [];
  public ICollection<RoleUser> RoleUsers { get; set; } = [];
}
