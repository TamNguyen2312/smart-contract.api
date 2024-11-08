using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.Manager;
using FS.Commons.Models;

namespace App.BLL.Implements;

public class ManagerBizLogic : IManagerBizLogic
{
    private readonly IManagerRepository _managerRepository;

    public ManagerBizLogic(IManagerRepository managerRepository)
    {
        _managerRepository = managerRepository;
    }

    public async Task<BaseResponse> CreateUpdateManager(ManagerRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var response = await _managerRepository.CreateUpdateManager(entity, userId);
        return response;
    }
}