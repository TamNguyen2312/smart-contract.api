using App.Entity.DTOs.Department;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface IDepartmentBizLogic
{
    Task<BaseResponse> CreateUpdateDepartment(DepartmentRequestDTO dto, long userId);
    Task<List<DepartmentViewDTO>> GetDropDownDepartment();
}