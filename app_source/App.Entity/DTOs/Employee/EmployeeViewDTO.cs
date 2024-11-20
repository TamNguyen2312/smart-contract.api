using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Models.DTOs;

namespace App.Entity.DTOs.Employee;

public class EmployeeViewDTO : UserViewDTO
{
    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public string CreatedBy { get; set; }
    public string? CreatedDate { get; set; }
    public string ModifiedBy { get; set; }
    public string? ModifiedDate { get; set; }
    public EmployeeViewDTO(ApplicationUser user, List<string> roles, Entities.Employee employee, Entities.Department department) : base(user, roles)
    {
        DepartmentId = department.Id;
        DepartmentName = department.Name;
        CreatedBy = employee.CreatedBy;
        CreatedDate = employee.CreatedDate.HasValue ? employee.CreatedDate.Value.ToString(Constants.FormatDate) : null;
        ModifiedBy = employee.ModifiedBy;
        ModifiedDate = employee.ModifiedDate.HasValue ? employee.ModifiedDate.Value.ToString(Constants.FormatDate) : null;
    }
}