using App.Entity.DTOs.Employee;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface IEmployeeBizLogic
{
    Task<BaseResponse> CreateUpdateEmployee(EmployeeRequestDTO dto, long userId);
}