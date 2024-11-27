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

    public async Task<List<ContractAppendixViewDTO>> GetContractAppendicesByContract(ContractAppendixGetListDTO dto, long contractId)
    {
        var data = await _contractAppendix.GetContractAppendicesByContract(dto, contractId);
        var response = data.Select(x => new ContractAppendixViewDTO(x)).ToList();
        return response;
    }

    public async Task<ContractAppendixViewDTO> GetContractAppendix(long contractId, long contractAppendixId)
    {
        var data = await _contractAppendix.GetContractAppendix(contractId, contractAppendixId);
        return new ContractAppendixViewDTO(data);
    }
}