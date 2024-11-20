using App.Entity.DTOs.Employee;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;

namespace App.BLL.Interfaces;

public interface IEmployeeBizLogic
{
    Task<BaseResponse> CreateUpdateEmployee(EmployeeRequestDTO dto, long userId);
    Task<EmployeeViewDTO> GetEmployee(long userId);
    Task<EmployeeViewDTO> GetEmployee(ApplicationUser user, List<string> userRoles);
    Task<List<EmployeeViewDTO>> GetAllEmployee(AccountGetListDTO dto);
}