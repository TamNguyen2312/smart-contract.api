using System.Text.Json;
using App.DAL.Interfaces;
using App.Entity.Entities;
using App.Entity.Enums;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;

namespace App.DAL.Implements;

public class AppendixDocumentRepository : IAppendixDocumentRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public AppendixDocumentRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<BaseResponse> CreateUpdateAppendixDocument(AppendixDocument appendixDocument, ApplicationUser user)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var baseAppendixDocumentRepo = _unitOfWork.GetRepository<AppendixDocument>();
            var baseSnapshotRepo = _unitOfWork.GetRepository<SnapshotMetadata>();
            var anyDocument = await baseAppendixDocumentRepo.AnyAsync(new QueryBuilder<AppendixDocument>()
                .WithPredicate(x => x.Id == appendixDocument.Id
                                    && !x.IsDelete)
                .Build());
            if (anyDocument)
            {
                var existedAppendixDocument = await baseAppendixDocumentRepo.GetSingleAsync(
                    new QueryBuilder<AppendixDocument>()
                        .WithPredicate(x => x.Id == appendixDocument.Id
                                            && !x.IsDelete)
                        .Build());

                appendixDocument.UpdateNonDefaultProperties(existedAppendixDocument);
                existedAppendixDocument.ModifiedDate = DateTime.Now;
                existedAppendixDocument.ModifiedBy = user.UserName;
                await baseAppendixDocumentRepo.UpdateAsync(existedAppendixDocument);

                var newSnapshot = new SnapshotMetadata
                {
                    Category = SnapshotMetadataType.AppendixDocument.ToString(),
                    CreatedDate = existedAppendixDocument.ModifiedDate,
                    CreatedBy = existedAppendixDocument.ModifiedBy,
                    Action = SnapshotMetadataAction.Update.ToString(),
                    IsDelete = false
                };
                newSnapshot.Name =
                    $"{existedAppendixDocument.Id}_{existedAppendixDocument.AppendixId}_{newSnapshot.Action}_{newSnapshot.CreatedBy}_{newSnapshot.CreatedDate}";
                newSnapshot.StoredData = JsonSerializer.Serialize(existedAppendixDocument);
                await baseSnapshotRepo.CreateAsync(newSnapshot);
            }
            else
            {
                var newAppendixDocument = new AppendixDocument
                {
                    Name = appendixDocument.Name,
                    Description = appendixDocument.Description,
                    FileName = appendixDocument.FileName,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.UserName,
                    AppendixId = appendixDocument.AppendixId,
                    IsDelete = false
                };

                await baseAppendixDocumentRepo.CreateAsync(newAppendixDocument);
                await _unitOfWork.SaveChangesAsync();


                var newSnapshot = new SnapshotMetadata
                {
                    Category = SnapshotMetadataType.AppendixDocument.ToString(),
                    CreatedDate = newAppendixDocument.CreatedDate,
                    CreatedBy = newAppendixDocument.CreatedBy,
                    Action = SnapshotMetadataAction.Create.ToString(),
                    IsDelete = false
                };
                newSnapshot.Name =
                    $"{newAppendixDocument.Id}_{newAppendixDocument.AppendixId}_{newSnapshot.Action}_{newSnapshot.CreatedBy}_{newSnapshot.CreatedDate}";
                newSnapshot.StoredData = JsonSerializer.Serialize(newAppendixDocument);
                await baseSnapshotRepo.CreateAsync(newSnapshot);
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
}