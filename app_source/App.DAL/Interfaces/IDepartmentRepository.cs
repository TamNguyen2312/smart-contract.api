using App.Entity.DTOs.Department;
using App.Entity.Entities;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface IDepartmentRepository
{
    Task<BaseResponse> CreateUpdateDepartment(Department department, long userId);
}