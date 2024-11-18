using System;
using App.DAL.Interfaces;
using App.Entity.Entities;
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
            var baseRepo = _unitOfWork.GetRepository<Contract>();
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
                CreatedBy = user.Email,
                CreatedDate = DateTime.Now
            };
            await baseRepo.CreateAsync(newContract);
            var saver = await _unitOfWork.SaveAsync();
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
