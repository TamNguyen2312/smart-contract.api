using System.Text.Json;
using App.DAL.Interfaces;
using App.Entity.DTOs.ContractTerm;
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

public class ContractTermRepository : IContractTermRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public ContractTermRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse> CreateUpdateContractTerm(ContractTerm contractTerm, ApplicationUser user)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var baseContractTermRepo = _unitOfWork.GetRepository<ContractTerm>();
            var baseSnapshotRepo = _unitOfWork.GetRepository<SnapshotMetadata>();
            var anyTerm = await baseContractTermRepo.AnyAsync(new QueryBuilder<ContractTerm>()
                .WithPredicate(x => x.Id == contractTerm.Id
                                    && !x.IsDelete)
                .Build());
            if (anyTerm)
            {
                var existedContractTerm = await baseContractTermRepo.GetSingleAsync(
                    new QueryBuilder<ContractTerm>()
                        .WithPredicate(x => x.Id == contractTerm.Id
                                            && !x.IsDelete)
                        .Build());

                contractTerm.UpdateNonDefaultProperties(existedContractTerm);
                existedContractTerm.ModifiedDate = DateTime.Now;
                existedContractTerm.ModifiedBy = user.UserName;
                await baseContractTermRepo.UpdateAsync(existedContractTerm);

                var newSnapshot = new SnapshotMetadata
                {
                    Category = SnapshotMetadataType.ContractTerm.ToString(),
                    CreatedDate = existedContractTerm.ModifiedDate,
                    CreatedBy = existedContractTerm.ModifiedBy,
                    Action = SnapshotMetadataAction.Update.ToString(),
                    IsDelete = false
                };
                newSnapshot.Name =
                    $"{existedContractTerm.Id}_{existedContractTerm.ContractId}_{newSnapshot.Action}_{newSnapshot.CreatedBy}_{newSnapshot.CreatedDate}";
                newSnapshot.StoredData = JsonSerializer.Serialize(existedContractTerm);
                await baseSnapshotRepo.CreateAsync(newSnapshot);
            }
            else
            {
                var newContractTerm = new ContractTerm
                {
                    Name = contractTerm.Name,
                    Description = contractTerm.Description,
                    ContractId = contractTerm.ContractId,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.UserName,
                    IsDelete = false
                };

                await baseContractTermRepo.CreateAsync(newContractTerm);
                await _unitOfWork.SaveChangesAsync();


                var newSnapshot = new SnapshotMetadata
                {
                    Category = SnapshotMetadataType.ContractTerm.ToString(),
                    CreatedDate = newContractTerm.CreatedDate,
                    CreatedBy = newContractTerm.CreatedBy,
                    Action = SnapshotMetadataAction.Create.ToString(),
                    IsDelete = false
                };
                newSnapshot.Name =
                    $"{newContractTerm.Id}_{newContractTerm.ContractId}_{newSnapshot.Action}_{newSnapshot.CreatedBy}_{newSnapshot.CreatedDate}";
                newSnapshot.StoredData = JsonSerializer.Serialize(newContractTerm);
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
    
    public async Task<List<ContractTerm>> GetContractTermsByContract(ContractTermGetListDTO dto, long contractId)
    {
        var baseContractTermRepo = _unitOfWork.GetRepository<ContractTerm>();
        var contractTerms = baseContractTermRepo.Get(new QueryBuilder<ContractTerm>()
            .WithPredicate(x => x.ContractId == contractId && !x.IsDelete)
            .Build());

        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            contractTerms = contractTerms.Where(x => x.Name.Contains(dto.Keyword)
                                                             || x.Description.Contains(dto.Keyword));
        }

        if (dto.OrderDate.HasValue)
        {
            contractTerms = contractTerms.ApplyOrderDate(dto.OrderDate);
        }

        contractTerms = contractTerms.ApplyDateRangeFilter(dto.CreatedDate, x => x.CreatedDate);
        contractTerms = contractTerms.ApplyDateRangeFilter(dto.ModifiedDate, x => x.ModifiedDate);

        dto.TotalRecord = await contractTerms.CountAsync();
        var result = await contractTerms.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return result;
    }
    
    public async Task<ContractTerm> GetContractTerm(long contractId, long contractTermId)
    {
        var baseContractTermRepo = _unitOfWork.GetRepository<ContractTerm>();
        var contractTerm = await baseContractTermRepo.GetSingleAsync(new QueryBuilder<ContractTerm>()
            .WithPredicate(x => x.Id == contractTermId
                                && x.ContractId == contractId
                                && !x.IsDelete)
            .Build());
        return contractTerm;
    }
}