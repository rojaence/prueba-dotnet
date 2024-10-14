using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Contexts;

public class ConnSqlServer(DbContextOptions<ConnSqlServer> options) : DbContext(options)
{
  public DbSet<User> Users { get; set; }
  public DbSet<Session> Sessions { get; set; }
  public DbSet<Role> Roles { get; set; }
  public DbSet<Permission> Permissions { get; set; }
  public DbSet<RolePermission> RolePermissions { get; set; }
  public DbSet<RoleUser> RoleUsers { get; set; }
  public DbSet<LoginAttempt> LoginAttempts { get; set; }

   public async Task<List<UserListItemDTO>> GetUsersWithDetailsAsync()
    {
        var users = new List<UserListItemDTO>();

        using (var command = Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "SELECT * FROM dbo.GetUsersWithLastSessionAndFirstRole()";
            command.CommandType = System.Data.CommandType.Text;

            await Database.OpenConnectionAsync();

      using var result = await command.ExecuteReaderAsync();
      while (await result.ReadAsync())
      {
        users.Add(new UserListItemDTO
        {
          IdUser = result.GetInt32(result.GetOrdinal("IdUser")),
          Username = result.GetString(result.GetOrdinal("Username")),
          SessionActive = result.GetBoolean(result.GetOrdinal("SessionActive")),
          Email = result.GetString(result.GetOrdinal("Email")),
          Status = result.GetBoolean(result.GetOrdinal("Status")),
          FirstName = result.GetString(result.GetOrdinal("FirstName")),
          MiddleName = result.GetString(result.GetOrdinal("MiddleName")),
          FirstLastname = result.GetString(result.GetOrdinal("FirstLastname")),
          SecondLastname = result.GetString(result.GetOrdinal("SecondLastname")),
          IdCard = result.GetString(result.GetOrdinal("IdCard")),
          RoleName = result.GetString(result.GetOrdinal("RoleName")),
        // Estos campo podrían ser nulos si no hay sesiones para un usuario. (por ejemplo: usuario nuevo)
          IdSession = result.IsDBNull(result.GetOrdinal("IdSession")) ? 0 : result.GetInt32(result.GetOrdinal("IdSession")),
          StartDate = result.IsDBNull(result.GetOrdinal("StartDate")) ? null : result.GetDateTime(result.GetOrdinal("StartDate")),

          /* IdSession = result.GetInt32(result.GetOrdinal("IdSession")),
          StartDate = result.GetDateTime(result.GetOrdinal("StartDate")),
          */
        });
      }
    }
    return users;
    }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
{

  // Claves primarias compuestas
    modelBuilder.Entity<RolePermission>()
      .HasKey(rp => new { rp.IdRole, rp.IdPermission });
    
    modelBuilder.Entity<RoleUser>()
      .HasKey(rp => new { rp.IdRole, rp.IdUser });

  // Claves primarias
    modelBuilder.Entity<User>()
        .HasKey(u => u.IdUser);

    modelBuilder.Entity<Session>()
        .HasKey(u => u.IdSession);

    modelBuilder.Entity<Role>()
        .HasKey(u => u.IdRole);
    
    modelBuilder.Entity<Permission>()
        .HasKey(u => u.IdPermission);

    modelBuilder.Entity<RolePermission>()
        .ToTable("Role_Permission");

    modelBuilder.Entity<RoleUser>()
        .ToTable("Role_User");

    modelBuilder.Entity<LoginAttempt>()
    .HasKey(u => u.IdAttempt);

    // RELATIONS
    modelBuilder.Entity<User>()
      .HasMany(u => u.Sessions)
      .WithOne(e => e.User)
      .HasForeignKey(e => e.IdUser);

    modelBuilder.Entity<User>()
      .HasMany(u => u.RoleUsers)
      .WithOne(e => e.User)
      .HasForeignKey(e => e.IdUser);


    modelBuilder.Entity<Role>()
        .HasMany(r => r.RoleUsers)
        .WithOne(ru => ru.Role)
        .HasForeignKey(ru => ru.IdRole);

    modelBuilder.Entity<Role>()
        .HasMany(r => r.RolePermissions)
        .WithOne(rp => rp.Role)
        .HasForeignKey(rp => rp.IdRole);
      
    modelBuilder.Entity<Permission>()
        .HasMany(p => p.RolePermissions)
        .WithOne(rp => rp.Permission)
        .HasForeignKey(rp => rp.IdPermission);
}
}

