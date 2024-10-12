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

