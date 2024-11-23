using System;
using System.Text.Json;
using App.DAL.Interfaces;
using App.Entity.DTOs.Contract;
using App.Entity.Entities;
using App.Entity.Enums;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implements;

public class ContractRepository : IContractRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public ContractRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse> CreateContract(Contract contract, ApplicationUser user, Employee employee)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var baseContractRepo = _unitOfWork.GetRepository<Contract>();
            var baseSnapshotRepo = _unitOfWork.GetRepository<SnapshotMetadata>();
            var baseEmpContractRepo = _unitOfWork.GetRepository<EmpContract>();
            var baseContractDepartmentRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();

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

            var newContractDepartment = new ContractDepartmentAssign
            {
                ContractId = newContract.Id,
                DepartmentId = employee.DepartmentId,
                CreatedBy = newContract.CreatedBy,
                CreatedDate = newContract.CreatedDate
            };
            await baseContractDepartmentRepo.CreateAsync(newContractDepartment);
            
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

    /// <summary>
    /// This is used to contracts that manager can access
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="managerId"></param>
    /// <returns></returns>
    public async Task<List<Contract>> GetContractsByManager(ContractGetListDTO dto, string managerId)
    {
        var baseManagerRepo = _unitOfWork.GetRepository<Manager>();
        var baseAssignRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();
        var baseContractRepo = _unitOfWork.GetRepository<Contract>();
        var managerDbSet = baseManagerRepo.GetDbSet();
        var assignDbSet = baseAssignRepo.GetDbSet();
        var contractDbSet = baseContractRepo.GetDbSet();
        
        var contracts = (from m in managerDbSet
                join cda in assignDbSet on m.DepartmentId equals cda.DepartmentId
                join c in contractDbSet on cda.ContractId equals c.Id
                where m.Id == managerId
                      && !m.IsDelete
                      && !cda.IsDelete
                      && !c.IsDelete
                select c)
            .AsNoTracking()
            .Distinct();

        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            contracts = contracts.Where(x => x.Title.Contains(dto.Keyword)
                                                    || x.KeyContent.Contains(dto.Keyword)
                                                    || x.ContractFile.Contains(dto.Keyword));
        }

        if (dto.OrderDate.HasValue)
        {
            contracts = contracts.ApplyOrderDate(dto.OrderDate);
        }

        if (dto.CustomerId.HasValue)
        {
            contracts = contracts.Where(x => x.CustomerId == dto.CustomerId.Value);
        }

        if (dto.ContractTypeId.HasValue)
        {
            contracts = contracts.Where(x => x.ContractTypeId == dto.ContractTypeId);
        }

        contracts = contracts.ApplyDateRangeFilter(dto.SignedDate, x => x.SignedDate);
        contracts = contracts.ApplyDateRangeFilter(dto.EffectiveDate, x => x.EffectiveDate);
        contracts = contracts.ApplyDateRangeFilter(dto.ExpirationDate, x => x.ExpirationDate);

        dto.TotalRecord = await contracts.CountAsync();
        var result = await contracts.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return result;
    }
}