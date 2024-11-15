using System;
using App.DAL.Interfaces;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;

namespace App.DAL.Implements;

public class FileUploadRepository : IFileUploadRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public FileUploadRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }
    public async Task<BaseResponse> CreateUpdateFileUpload(FileUpload fileUpload, ApplicationUser user)
    {
        try
        {
            var baseRepo = _unitOfWork.GetRepository<FileUpload>();
            await _unitOfWork.BeginTransactionAsync();

            var any = await baseRepo.AnyAsync(new QueryBuilder<FileUpload>()
                                            .WithPredicate(x => x.Id == fileUpload.Id)
                                            .Build());
            if (any)
            {
                var existed = await baseRepo.GetSingleAsync(new QueryBuilder<FileUpload>()
                                                        .WithPredicate(x => x.Id == fileUpload.Id)
                                                        .Build());
                if (existed.UserId != fileUpload.UserId) return new BaseResponse { IsSuccess = false, Message = Constants.UserNotSame };
                fileUpload.UpdateNonDefaultProperties(existed);
                existed.ModifiedBy = user.Email;
                existed.ModifiedDate = DateTime.Now;
                await baseRepo.UpdateAsync(existed);
            }
            else
            {
                var newFileUpload = new FileUpload
                {
                    FileName = fileUpload.FileName,
                    FilePath = fileUpload.FilePath,
                    UserId = fileUpload.UserId,
                    CreatedBy = user.Email,
                    CreatedDate = DateTime.Now,
                    IsDelete = false
                };
                await baseRepo.CreateAsync(newFileUpload);
            }
            var saver = await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitTransactionAsync();
            if (!saver) return new BaseResponse { IsSuccess = false, Message = Constants.SaveDataFailed };
            return new BaseResponse { IsSuccess = true, Message = Constants.SaveDataSuccess };
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
    }

    public async Task<FileUpload> GetFileUploadByFilePath(string storagePath, string safeFileName)
    {
        var baseRepo = _unitOfWork.GetRepository<FileUpload>();
        var file = await baseRepo.GetSingleAsync(new QueryBuilder<FileUpload>()
                                                .WithPredicate(x => x.FileName.Equals(safeFileName)
                                                                && x.FilePath.Equals(storagePath)
                                                                && x.IsDelete == false)
                                                .Build());
        return file;
    }
}
