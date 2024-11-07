using FS.BaseModels.IdentityModels;

namespace App.Entity.DTOs.Department;

public class DepartmentViewDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }

    public DepartmentViewDTO(Entities.Department department, ApplicationUser user)
    {
        Id = department.Id;
        Name = department.Name;
        Description = department.Description;
        CreatedBy = user.UserName;
        CreatedDate = department.CreatedDate;
        ModifiedBy = user.UserName;
        ModifiedDate = department.ModifiedDate;
    }
}