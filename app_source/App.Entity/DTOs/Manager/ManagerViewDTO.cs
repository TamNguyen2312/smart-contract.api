using FS.BaseModels.IdentityModels;
using FS.Commons.Models.DTOs;

namespace App.Entity.DTOs.Manager;

public class ManagerViewDTO : UserViewDTO
{
    public string DepartmentName { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public ManagerViewDTO(ApplicationUser user, List<string> roles, Entities.Manager manager, Entities.Department department) : base(user, roles)
    {
        DepartmentName = department.Name;
        CreatedBy = manager.CreatedBy;
        CreatedDate = manager.CreatedDate;
        ModifiedBy = manager.ModifiedBy;
        ModifiedDate = manager.ModifiedDate;
    }
}