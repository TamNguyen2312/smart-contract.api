using FS.BaseModels.IdentityModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FS.IdentityFramework;

public partial class FSDbContext : IdentityDbContext<ApplicationUser, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    public FSDbContext(DbContextOptions<FSDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("dbo");
        
        modelBuilder.Entity<ApplicationUser>(b =>
        {
            b.ToTable("FS_Users");
            // Each User can have many UserClaims
            b.HasMany(e => e.Claims)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            // Each User can have many UserLogins
            b.HasMany(e => e.Logins)
                .WithOne(e => e.User)
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            b.HasMany(e => e.Tokens)
                .WithOne(e => e.User)
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            b.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        });

        modelBuilder.Entity<UserClaim>(b =>
        {
            b.ToTable("FS_UserClaims");
        });

        modelBuilder.Entity<UserLogin>(b =>
        {
            b.ToTable("FS_UserLogins");
        });

        modelBuilder.Entity<UserToken>(b =>
        {
            b.ToTable("FS_UserTokens");
        });

        modelBuilder.Entity<Role>(b =>
        {
            b.ToTable("FS_Roles");
            // Each Role can have many entries in the UserRole join table
            b.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            // Each Role can have many associated RoleClaims
            b.HasMany(e => e.RoleClaims)
                .WithOne(e => e.Role)
                .HasForeignKey(rc => rc.RoleId)
                .IsRequired();
        });

        modelBuilder.Entity<RoleClaim>(b =>
        {
            b.ToTable("FS_RoleClaims");
        });

        modelBuilder.Entity<UserRole>(b =>
        {
            b.ToTable("FS_UserRoles");
        });
        
        OnModelCreatingPartial(modelBuilder);
    }
    
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}