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

  protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>()
        .HasKey(u => u.IdUser);

    modelBuilder.Entity<Session>()
        .HasKey(u => u.IdSession);

    modelBuilder.Entity<Role>()
        .HasKey(u => u.IdRole);
    
    modelBuilder.Entity<Permission>()
        .HasKey(u => u.IdPermission);

    modelBuilder.Entity<RolePermission>()
        .ToTable("Role_Permission")
        .HasNoKey();

    modelBuilder.Entity<RoleUser>()
        .ToTable("Role_User")
        .HasNoKey();

    modelBuilder.Entity<LoginAttempt>()
    .HasKey(u => u.IdAttempt);
}
}

