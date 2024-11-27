using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.AppendixDocument;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class AppendixDocumentBizLogic : IAppendixDocumentBizLogic
{
    private readonly IAppendixDocumentRepository _appendixDocumentRepository;
    private readonly IIdentityRepository _identityRepository;

    public AppendixDocumentBizLogic(IAppendixDocumentRepository appendixDocumentRepository, IIdentityRepository identityRepository)
    {
        _appendixDocumentRepository = appendixDocumentRepository;
        _identityRepository = identityRepository;
    }

    public async Task<BaseResponse> CreateUpdateAppendixDocument(AppendixDocumentRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _appendixDocumentRepository.CreateUpdateAppendixDocument(entity, user);
        return response;
    }
}