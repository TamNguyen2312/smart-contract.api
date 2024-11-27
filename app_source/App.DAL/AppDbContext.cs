using App.Entity.Entities;
using App.Entity.Enums;
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
    public virtual DbSet<SnapshotMetadata> SnapshotMetadatas { get; set; }
    public virtual DbSet<ContractDepartmentAssign> ContractDepartmentAssigns { get; set; }
    public virtual DbSet<ContractDocument> ContractDocuments { get; set; }
    public virtual DbSet<ContractTerm> ContractTerms { get; set; }
    public virtual DbSet<ContractAppendix> ContractAppendices { get; set; }
    public virtual DbSet<AppendixDocument> AppendicDocuments { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("app");
        
        modelBuilder.Entity<AppendixDocument>(ad =>
        {
            ad.ToTable("App_AppendixDocument");
            ad.HasKey(appendixDocument => appendixDocument.Id);
        });
        
        modelBuilder.Entity<ContractAppendix>(ca =>
        {
            ca.ToTable("App_ContractAppendices");
            ca.HasKey(contractAppendix => contractAppendix.Id);
        });

        modelBuilder.Entity<ContractTerm>(cd =>
        {
            cd.ToTable("App_ContractTerms");
            cd.HasKey(contractTerm => contractTerm.Id);
        });
        
        modelBuilder.Entity<ContractDocument>(cd =>
        {
            cd.ToTable("App_ContractDocuments");
            cd.HasKey(contractDocument => contractDocument.Id);
        });
        
        modelBuilder.Entity<ContractDepartmentAssign>(cda =>
        {
            cda.ToTable("App_ContractDepartmentAssigns");
            cda.HasKey(assign => new {assign.ContractId, assign.DepartmentId});
        });
        
        modelBuilder.Entity<SnapshotMetadata>(sm =>
        {
            sm.ToTable("App_SnapshotMetadata");
            sm.HasKey(metadata => metadata.Id);
        });

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