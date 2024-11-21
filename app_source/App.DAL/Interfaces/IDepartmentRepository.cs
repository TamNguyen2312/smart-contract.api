using App.Entity.DTOs.Department;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface IDepartmentRepository
{
    Task<BaseResponse> CreateUpdateDepartment(Department department, ApplicationUser user);
    Task<List<Department>> GetDropDownDepartment();
    Task<Department> GetDepartment(long id);
    Task<List<Department>> GetAllDepartments(DepartmentGetListDTO dto);
}