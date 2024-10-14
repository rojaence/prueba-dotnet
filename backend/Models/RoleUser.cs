using System.Text.Json.Serialization;

namespace backend.Models;

public class RoleUser 
{
  public int IdRole { get; set; }
  public Role Role { get; set; }
  public int IdUser { get; set; }
  [JsonIgnore]
  public User User { get; set; }
}