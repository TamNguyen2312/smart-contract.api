using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.ContractDocument;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class ContractDocumentBizLogic : IContractDocumentBizLogic
{
    private readonly IContractDocumentRepository _contractDocumentRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IIdentityRepository _identityRepository;

    public ContractDocumentBizLogic(IContractDocumentRepository contractDocumentRepository,
                                    IContractRepository contractRepository,
                                    IIdentityRepository identityRepository)
    {
        _contractDocumentRepository = contractDocumentRepository;
        _contractRepository = contractRepository;
        _identityRepository = identityRepository;
    }

    public async Task<BaseResponse> CreateUpdateContractDocument(ContractDocumentRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _contractDocumentRepository.CreateUpdateContractDocument(entity, user);
        return response;
    }
}