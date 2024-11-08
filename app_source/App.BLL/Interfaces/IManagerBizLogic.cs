using App.Entity.DTOs.Manager;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface IManagerBizLogic
{
    Task<BaseResponse> CreateUpdateManager(ManagerRequestDTO dto, long userId);
}