using App.Entity.DTOs.Department;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface IDepartmentBizLogic
{
    Task<BaseResponse> CreateUpdateDepartment(DepartmentRequestDto dto, long userId);
    Task<List<DepartmentViewDTO>> GetDropDownDepartment();

    Task<DepartmentViewDTO> GetDepartment(long id);
    Task<List<DepartmentViewDTO>> GetAllDepartments(DepartmentGetListDTO dto);
}