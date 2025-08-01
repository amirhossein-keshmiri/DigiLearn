﻿using Microsoft.EntityFrameworkCore;
using UserModule.Data.Entities.Roles;
using UserModule.Data.Entities.Users;

namespace UserModule.Data.Entities
{
  public class UserContext : DbContext
  {
    public UserContext(DbContextOptions<UserContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
  }
}
