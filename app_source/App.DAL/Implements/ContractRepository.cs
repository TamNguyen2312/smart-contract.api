using System;
using System.Text.Json;
using App.DAL.Interfaces;
using App.Entity.Entities;
using App.Entity.Enums;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Models;
using FS.DAL.Interfaces;

namespace App.DAL.Implements;

public class ContractRepository : IContractRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public ContractRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse> CreateContract(Contract contract, ApplicationUser user)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var baseContractRepo = _unitOfWork.GetRepository<Contract>();
            var baseSnapshotRepo = _unitOfWork.GetRepository<SnapshotMetadata>();
            var baseEmpContractRepo = _unitOfWork.GetRepository<EmpContract>();

            var newContract = new Contract
            {
                Title = contract.Title,
                SignedDate = contract.SignedDate,
                EffectiveDate = contract.EffectiveDate,
                ExpirationDate = contract.ExpirationDate,
                KeyContent = contract.KeyContent,
                ContractFile = contract.ContractFile,
                CustomerId = contract.CustomerId,
                ContractTypeId = contract.ContractTypeId,
                ContractDaysLeft = contract.ContractDaysLeft,
                AppendixDaysLeft = contract.AppendixDaysLeft,
                CreatedBy = user.UserName,
                CreatedDate = DateTime.Now,
                IsDelete = false
            };
            await baseContractRepo.CreateAsync(newContract);
            await _unitOfWork.SaveChangesAsync();

            var newEmpContract = new EmpContract
            {
                ContractId = newContract.Id,
                EmployeeId = user.Id.ToString(),
                Description =
                    $"Nhân viên {user.UserName} tạo hợp đồng {newContract.Id} vào {newContract.CreatedDate.Value.ToString(Constants.FormatFullDateTime)}",
                CreatedBy = newContract.CreatedBy,
                CreatedDate = newContract.CreatedDate,
                IsDelete = false
            };
            await baseEmpContractRepo.CreateAsync(newEmpContract);

            var newSnapshot = new SnapshotMetadata
            {
                Category = SnapshotMetadataType.Contract.ToString(),
                CreatedDate = newContract.CreatedDate,
                CreatedBy = newContract.CreatedBy,
                Action = SnapshotMetadataAction.Create.ToString(),
                IsDelete = false
            };
            newSnapshot.Name = $"{newSnapshot.Category}_{newSnapshot.Action}_{newSnapshot.CreatedBy}_{newSnapshot.CreatedDate}";
            newSnapshot.StoredData = JsonSerializer.Serialize(newContract);
            await baseSnapshotRepo.CreateAsync(newSnapshot);

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