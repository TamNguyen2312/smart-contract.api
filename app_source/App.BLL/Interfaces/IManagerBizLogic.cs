using App.Entity.DTOs.Manager;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;

namespace App.BLL.Interfaces;

public interface IManagerBizLogic
{
    Task<BaseResponse> CreateUpdateManager(ManagerRequestDTO dto, long userId);
    Task<ManagerViewDTO> GetManager(long userId);
    Task<ManagerViewDTO> GetManager(ApplicationUser user, List<string> userRolse);
    Task<List<ManagerViewDTO>> GetAllManager(AccountGetListDTO dto);

    /// <summary>
    /// This is used to get a manager of a department
    /// </summary>
    /// <param name="departmentId"></param>
    /// <returns></returns>
    Task<ManagerViewDTO> GetManagerByDepartmentId(long departmentId);
}