using App.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.DAL;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    #region AppDbSet

    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("app");

        modelBuilder.Entity<Customer>(c =>
        {
            c.ToTable("App_Customers");
            c.HasKey(c => c.Id);
        });

        modelBuilder.Entity<Employee>(e =>
        {
            e.ToTable("App_Employees");
            e.HasKey(x => x.Id);
        });
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}