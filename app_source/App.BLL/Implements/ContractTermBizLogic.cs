using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.ContractTerm;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class ContractTermBizLogic : IContractTermBizLogic
{
    private readonly IContractTermRepository _contractTermRepository;
    private readonly IIdentityRepository _identityRepository;

    public ContractTermBizLogic(IContractTermRepository contractTermRepository, IIdentityRepository identityRepository)
    {
        _contractTermRepository = contractTermRepository;
        _identityRepository = identityRepository;
    }

    public async Task<BaseResponse> CreateUpdateContractTerm(ContractTermRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _contractTermRepository.CreateUpdateContractTerm(entity, user);
        return response;
    }

    public async Task<List<ContractTermViewDTO>> GetContractTermsByContract(ContractTermGetListDTO dto, long contractId)
    {
        var data = await _contractTermRepository.GetContractTermsByContract(dto, contractId);
        return data.Select(x => new ContractTermViewDTO(x)).ToList();
    }
}