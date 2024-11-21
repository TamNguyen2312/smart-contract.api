using FS.BaseModels.IdentityModels;
using FS.Commons;

namespace App.Entity.DTOs.Department;

public class DepartmentViewDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public int EmployeeQuantity { get; set; }
    public int MornitorQuantity { get; set; }
    public string? Description { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public string? ModifiedDate { get; set; }

    public DepartmentViewDTO(Entities.Department department, ApplicationUser user)
    {
        Id = department.Id;
        Name = department.Name;
        EmployeeQuantity = department.EmployeeQuantity;
        MornitorQuantity = department.MornitorQuantity;
        Description = department.Description;
        CreatedBy = department.CreatedBy;
        CreatedDate = department.CreatedDate.HasValue ? department.CreatedDate.Value.ToString(Constants.FormatDate) : null;
        ModifiedBy = department.ModifiedBy;
        ModifiedDate = department.ModifiedDate.HasValue ? department.ModifiedDate.Value.ToString(Constants.FormatDate) : null;
    }
}