using System;
using App.Entity.DTOs.File;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface IFileUploadBizLogic
{
    Task<BaseResponse> CreateUpdateFileUpload(FileUploadRequestDTO dto, long userId);
    Task<FileUploadViewDTO> GetFileUploadByFilePath(string storagePath, string safeFileName);
}
