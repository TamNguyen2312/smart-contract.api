using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.Department;
using FS.Commons.Models;

namespace App.BLL.Implements;

public class DepartmentBizLogic : IDepartmentBizLogic
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentBizLogic(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }
    
    
    public async Task<BaseResponse> CreateUpdateDepartment(DepartmentRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var response = await _departmentRepository.CreateUpdateDepartment(entity, userId);
        return response;
    }
}