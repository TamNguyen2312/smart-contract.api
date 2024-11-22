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
    public virtual DbSet<ContractType> ContractTypes { get; set; }
    public virtual DbSet<Contract> Contracts { get; set; }
    public virtual DbSet<CustomerDocument> CustomerDocuments { get; set; }
    public virtual DbSet<CustomerDepartmentAssign> CustomerDepartmentAssigns { get; set; }
    public virtual DbSet<EmpContract> EmpContractss { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("app");

        modelBuilder.Entity<EmpContract>(ec =>
        {
            ec.ToTable("App_EmpContracts");
            ec.HasKey(empContract => new {empContract.EmployeeId, empContract.ContractId});
        });
        
        modelBuilder.Entity<CustomerDepartmentAssign>(c =>
        {
            c.ToTable("App_CustomerDeparmentAssigns");
            c.HasKey(cd => cd.Id);
        });
        
        modelBuilder.Entity<CustomerDocument>(c =>
        {
            c.ToTable("App_CustomerDocuments");
            c.HasKey(cd => cd.Id);
        });
        
        modelBuilder.Entity<Contract>(c =>
        {
            c.ToTable("App_Contracts");
            c.HasKey(c => c.Id);
        });

        modelBuilder.Entity<ContractType>(c =>
        {
            c.ToTable("App_ContractTypes");
            c.HasKey(f => f.Id);
        });

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