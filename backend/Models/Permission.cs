namespace backend.Models;

public class Permission
{
  public int IdPermission { get; set; }
  public string? Name { get; set; }
  public ICollection<RolePermission> RolePermissions { get; set; }= new List<RolePermission>();
}

public class PermissionDTO
{
  public int IdPermission { get; set; }
  public string? Name { get; set; }
}