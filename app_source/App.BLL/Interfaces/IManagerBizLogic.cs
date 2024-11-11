using App.Entity.DTOs.Manager;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface IManagerBizLogic
{
    Task<BaseResponse> CreateUpdateManager(ManagerRequestDTO dto, long userId);
    Task<ManagerViewDTO> GetManager(long userId);
    Task<ManagerViewDTO> GetManager(ApplicationUser user, List<string> userRolse);
}