using App.Entity.DTOs.Employee;
using App.Entity.Entities;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface IEmployeeRepository
{
    Task<BaseResponse> CreateUpdateEmployee(Employee emp, long userId);
    Task<Employee> GetEmployee(long userId);
    Task<Employee> GetEmployeeForAdmin(long empId);
}