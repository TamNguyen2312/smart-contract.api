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
    public virtual DbSet<Manager> Managers { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<FileUpload> FileUploads { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("app");

        modelBuilder.Entity<FileUpload>(c =>
        {
            c.ToTable("App_FileUploads");
            c.HasKey(f => f.Id);
        });

        modelBuilder.Entity<Department>(c =>
        {
            c.ToTable("App_Departments");
            c.HasKey(m => m.Id);
        });

        modelBuilder.Entity<Manager>(c =>
        {
            c.ToTable("App_Managers");
            c.HasKey(m => m.Id);
        });

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