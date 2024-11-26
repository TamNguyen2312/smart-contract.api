using System.Text.Json;
using App.DAL.Interfaces;
using App.Entity.DTOs.ContractDocument;
using App.Entity.Entities;
using App.Entity.Enums;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implements;

public class ContractDocumentRepository : IContractDocumentRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public ContractDocumentRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse> CreateUpdateContractDocument(ContractDocument contractDocument,
        ApplicationUser user)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var baseContractDocumentRepo = _unitOfWork.GetRepository<ContractDocument>();
            var baseSnapshotRepo = _unitOfWork.GetRepository<SnapshotMetadata>();
            var anyDocument = await baseContractDocumentRepo.AnyAsync(new QueryBuilder<ContractDocument>()
                .WithPredicate(x => x.Id == contractDocument.Id
                                    && !x.IsDelete)
                .Build());
            if (anyDocument)
            {
                var existedContractDocument = await baseContractDocumentRepo.GetSingleAsync(
                    new QueryBuilder<ContractDocument>()
                        .WithPredicate(x => x.Id == contractDocument.Id
                                            && !x.IsDelete)
                        .Build());

                contractDocument.UpdateNonDefaultProperties(existedContractDocument);
                existedContractDocument.ModifiedDate = DateTime.Now;
                existedContractDocument.ModifiedBy = user.UserName;
                await baseContractDocumentRepo.UpdateAsync(existedContractDocument);

                var newSnapshot = new SnapshotMetadata
                {
                    Category = SnapshotMetadataType.ContractDocument.ToString(),
                    CreatedDate = existedContractDocument.ModifiedDate,
                    CreatedBy = existedContractDocument.ModifiedBy,
                    Action = SnapshotMetadataAction.Update.ToString(),
                    IsDelete = false
                };
                newSnapshot.Name =
                    $"{existedContractDocument.Id}_{existedContractDocument.ContractId}_{newSnapshot.Action}_{newSnapshot.CreatedBy}_{newSnapshot.CreatedDate}";
                newSnapshot.StoredData = JsonSerializer.Serialize(existedContractDocument);
                await baseSnapshotRepo.CreateAsync(newSnapshot);
            }
            else
            {
                var newContractDocument = new ContractDocument
                {
                    Name = contractDocument.Name,
                    Description = contractDocument.Description,
                    FileName = contractDocument.FileName,
                    ContractId = contractDocument.ContractId,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.UserName,
                    IsDelete = false
                };

                await baseContractDocumentRepo.CreateAsync(newContractDocument);
                await _unitOfWork.SaveChangesAsync();


                var newSnapshot = new SnapshotMetadata
                {
                    Category = SnapshotMetadataType.ContractDocument.ToString(),
                    CreatedDate = newContractDocument.CreatedDate,
                    CreatedBy = newContractDocument.CreatedBy,
                    Action = SnapshotMetadataAction.Create.ToString(),
                    IsDelete = false
                };
                newSnapshot.Name =
                    $"{newContractDocument.Id}_{newContractDocument.ContractId}_{newSnapshot.Action}_{newSnapshot.CreatedBy}_{newSnapshot.CreatedDate}";
                newSnapshot.StoredData = JsonSerializer.Serialize(newContractDocument);
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
    
    public async Task<List<ContractDocument>> GetContractDocumentsByContract(ContractDocumentGetListDTO dto, long contractId)
    {
        var baseContractDocumentRepo = _unitOfWork.GetRepository<ContractDocument>();
        var contractDocuments = baseContractDocumentRepo.Get(new QueryBuilder<ContractDocument>()
            .WithPredicate(x => x.ContractId == contractId && !x.IsDelete)
            .Build());

        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            contractDocuments = contractDocuments.Where(x => x.Name.Contains(dto.Keyword)
                                                             || x.Description.Contains(dto.Keyword)
                                                             || x.FileName.Contains(dto.Keyword));
        }

        if (dto.OrderDate.HasValue)
        {
            contractDocuments = contractDocuments.ApplyOrderDate(dto.OrderDate);
        }

        contractDocuments = contractDocuments.ApplyDateRangeFilter(dto.CreatedDate, x => x.CreatedDate);
        contractDocuments = contractDocuments.ApplyDateRangeFilter(dto.ModifiedDate, x => x.ModifiedDate);

        dto.TotalRecord = await contractDocuments.CountAsync();
        var result = await contractDocuments.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return result;
    }
}