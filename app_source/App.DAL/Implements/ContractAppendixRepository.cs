using System.Text.Json;
using App.DAL.Interfaces;
using App.Entity.DTOs.ContractAppendix;
using App.Entity.Entities;
using App.Entity.Enums;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implements;

public class ContractAppendixRepository : IContractAppendixRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public ContractAppendixRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse> CreateUpdateContractAppendix(ContractAppendix contractAppendix, ApplicationUser user)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var baseContractAppendixRepo = _unitOfWork.GetRepository<ContractAppendix>();
            var baseSnapshotRepo = _unitOfWork.GetRepository<SnapshotMetadata>();
            var anyAppendix = await baseContractAppendixRepo.AnyAsync(new QueryBuilder<ContractAppendix>()
                .WithPredicate(x => x.Id == contractAppendix.Id
                                    && !x.IsDelete)
                .Build());
            if (anyAppendix)
            {
                var existedContractAppendix = await baseContractAppendixRepo.GetSingleAsync(
                    new QueryBuilder<ContractAppendix>()
                        .WithPredicate(x => x.Id == contractAppendix.Id
                                            && !x.IsDelete)
                        .Build());

                contractAppendix.UpdateNonDefaultProperties(existedContractAppendix);
                existedContractAppendix.ModifiedDate = DateTime.Now;
                existedContractAppendix.ModifiedBy = user.UserName;
                await baseContractAppendixRepo.UpdateAsync(existedContractAppendix);

                var newSnapshot = new SnapshotMetadata
                {
                    Category = SnapshotMetadataType.Appendix.ToString(),
                    CreatedDate = existedContractAppendix.ModifiedDate,
                    CreatedBy = existedContractAppendix.ModifiedBy,
                    Action = SnapshotMetadataAction.Update.ToString(),
                    IsDelete = false
                };
                newSnapshot.Name =
                    $"{existedContractAppendix.Id}_{existedContractAppendix.ContractId}_{newSnapshot.Action}_{newSnapshot.CreatedBy}_{newSnapshot.CreatedDate}";
                newSnapshot.StoredData = JsonSerializer.Serialize(existedContractAppendix);
                await baseSnapshotRepo.CreateAsync(newSnapshot);
            }
            else
            {
                var newContractAppendix = new ContractAppendix
                {
                    Title = contractAppendix.Title,
                    Content = contractAppendix.Content,
                    ContractId = contractAppendix.ContractId,
                    SignedDate = contractAppendix.SignedDate,
                    EffectiveDate = contractAppendix.EffectiveDate,
                    ExpirationDate = contractAppendix.ExpirationDate,
                    FileName = contractAppendix.FileName,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.UserName,
                    IsDelete = false
                };

                await baseContractAppendixRepo.CreateAsync(newContractAppendix);
                await _unitOfWork.SaveChangesAsync();


                var newSnapshot = new SnapshotMetadata
                {
                    Category = SnapshotMetadataType.Appendix.ToString(),
                    CreatedDate = newContractAppendix.CreatedDate,
                    CreatedBy = newContractAppendix.CreatedBy,
                    Action = SnapshotMetadataAction.Create.ToString(),
                    IsDelete = false
                };
                newSnapshot.Name =
                    $"{newContractAppendix.Id}_{newContractAppendix.ContractId}_{newSnapshot.Action}_{newSnapshot.CreatedBy}_{newSnapshot.CreatedDate}";
                newSnapshot.StoredData = JsonSerializer.Serialize(newContractAppendix);
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
    
    public async Task<List<ContractAppendix>> GetContractAppendicesByContract(ContractAppendixGetListDTO dto, long contractId)
    {
        var baseContractAppendixRepo = _unitOfWork.GetRepository<ContractAppendix>();
        var contractAppendices = baseContractAppendixRepo.Get(new QueryBuilder<ContractAppendix>()
            .WithPredicate(x => x.ContractId == contractId && !x.IsDelete)
            .Build());

        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            contractAppendices = contractAppendices.Where(x => x.Title.Contains(dto.Keyword)
                                                     || x.Content.Contains(dto.Keyword)
                                                     || x.FileName.Contains(dto.Keyword));
        }

        if (dto.OrderDate.HasValue)
        {
            contractAppendices = contractAppendices.ApplyOrderDate(dto.OrderDate);
        }

        contractAppendices = contractAppendices.ApplyDateRangeFilter(dto.SignedDate, x => x.SignedDate);
        contractAppendices = contractAppendices.ApplyDateRangeFilter(dto.EffectiveDate, x => x.EffectiveDate);
        contractAppendices = contractAppendices.ApplyDateRangeFilter(dto.ExpirationDate, x => x.ExpirationDate);
        contractAppendices = contractAppendices.ApplyDateRangeFilter(dto.CreatedDate, x => x.CreatedDate);
        contractAppendices = contractAppendices.ApplyDateRangeFilter(dto.ModifiedDate, x => x.ModifiedDate);

        dto.TotalRecord = await contractAppendices.CountAsync();
        var result = await contractAppendices.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return result;
    }
}