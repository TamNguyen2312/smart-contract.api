using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models.DTOs;

namespace App.Entity.DTOs.Employee;

public class EmployeeViewDTO : UserViewDTO
{
    public string DepartmentName { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public EmployeeViewDTO(ApplicationUser user, List<string> roles, Entities.Employee employee, Department department) : base(user, roles)
    {
        DepartmentName = department.Name;
        CreatedBy = employee.CreatedBy;
        CreatedDate = employee.CreatedDate;
        ModifiedBy = employee.ModifiedBy;
        ModifiedDate = employee.ModifiedDate;
    }
}