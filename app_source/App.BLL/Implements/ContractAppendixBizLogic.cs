using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.ContractAppendix;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class ContractAppendixBizLogic : IContractAppendixBizLogic
{
    private readonly IContractAppendixRepository _contractAppendix;
    private readonly IIdentityRepository _identityRepository;

    public ContractAppendixBizLogic(IContractAppendixRepository contractAppendix, IIdentityRepository identityRepository)
    {
        _contractAppendix = contractAppendix;
        _identityRepository = identityRepository;
    }

    public async Task<BaseResponse> CreateUpdateContractAppendix(ContractAppendixRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _contractAppendix.CreateUpdateContractAppendix(entity, user);
        return response;
    }
}