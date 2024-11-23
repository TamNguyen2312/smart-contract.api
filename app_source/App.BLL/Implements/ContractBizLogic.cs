using System;
using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.Contract;
using FS.Commons.Models;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class ContractBizLogic : IContractBizLogic
{
    private readonly IContractRepository _contractRepository;
    private readonly IIdentityRepository _identityRepository;

    public ContractBizLogic(IContractRepository contractRepository, IIdentityRepository identityRepository)
    {
        _contractRepository = contractRepository;
        _identityRepository = identityRepository;
    }

    public async Task<BaseResponse> CreateContract(ContractRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _contractRepository.CreateContract(entity, user);
        return response;
    }
}
