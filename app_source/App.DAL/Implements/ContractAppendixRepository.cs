using App.DAL.Interfaces;
using App.Entity.Entities;
using App.Entity.Enums;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;

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
                var existedContractTerm = await baseContractAppendixRepo.GetSingleAsync(
                    new QueryBuilder<ContractAppendix>()
                        .WithPredicate(x => x.Id == contractAppendix.Id
                                            && !x.IsDelete)
                        .Build());

                contractTerm.UpdateNonDefaultProperties(existedContractTerm);
                existedContractTerm.ModifiedDate = DateTime.Now;
                existedContractTerm.ModifiedBy = user.UserName;
                await baseContractAppendixRepo.UpdateAsync(existedContractTerm);

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

                await baseContractAppendixRepo.CreateAsync(newContractTerm);
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
}