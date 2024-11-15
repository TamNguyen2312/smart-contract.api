using System;
using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.File;
using FS.Commons.Models;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class FileUploadBizLogic : IFileUploadBizLogic
{
    private readonly IFileUploadRepository _fileUploadRepository;
    private readonly IIdentityRepository _identityRepository;

    public FileUploadBizLogic(IFileUploadRepository fileUploadRepository, IIdentityRepository identityRepository)
    {
        this._fileUploadRepository = fileUploadRepository;
        this._identityRepository = identityRepository;
    }
    public async Task<BaseResponse> CreateUpdateFileUpload(FileUploadRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _fileUploadRepository.CreateUpdateFileUpload(entity, user);
        return response;
    }
}
