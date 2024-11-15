using System;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface IFileUploadRepository
{
    Task<BaseResponse> CreateUpdateFileUpload(FileUpload fileUpload, ApplicationUser user);
    Task<FileUpload> GetFileUploadByFilePath(string storagePath, string safeFileName);
}
