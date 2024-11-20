using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Models.DTOs;

namespace App.Entity.DTOs.Manager;

public class ManagerViewDTO : UserViewDTO
{
    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public string CreatedBy { get; set; }
    public string? CreatedDate { get; set; }
    public string ModifiedBy { get; set; }
    public string? ModifiedDate { get; set; }
    public ManagerViewDTO(ApplicationUser user, List<string> roles, Entities.Manager manager, Entities.Department department) : base(user, roles)
    {
        DepartmentId = department.Id;
        DepartmentName = department.Name;
        CreatedBy = manager.CreatedBy;
        CreatedDate = manager.CreatedDate.HasValue ? manager.CreatedDate.Value.ToString(Constants.FormatDate) : null;
        ModifiedBy = manager.ModifiedBy;
        ModifiedDate = manager.ModifiedDate.HasValue ? manager.ModifiedDate.Value.ToString(Constants.FormatDate) : null;
    }
}