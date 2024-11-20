using App.Entity.DTOs.Employee;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;

namespace App.DAL.Interfaces;

public interface IEmployeeRepository
{
    Task<BaseResponse> CreateUpdateEmployee(Employee emp, ApplicationUser user);
    Task<Employee> GetEmployee(long userId);
    Task<Employee> GetEmployeeForAdmin(long empId);
    Task<List<UserEmpDTO>> GetAllEmployee(AccountGetListDTO dto);
}