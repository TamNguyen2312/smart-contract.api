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
using FS.DAL.Queries;
using Microsoft.AspNetCore.Http.Extensions;
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
            newSnapshot.Name =
                $"{newContract.Id}_{newSnapshot.Action}_{newSnapshot.CreatedBy}_{newSnapshot.CreatedDate}";
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

    public async Task<BaseResponse> UpdateContract(Contract contract, ApplicationUser user)
    {
        try
        {
            var baseContractRepo = _unitOfWork.GetRepository<Contract>();
            var baseSnapshotRepo = _unitOfWork.GetRepository<SnapshotMetadata>();
            var baseEmpContractRepo = _unitOfWork.GetRepository<EmpContract>();
            var baseContractDepartmentRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();

            var anyContract = await baseContractRepo.AnyAsync(new QueryBuilder<Contract>()
                .WithPredicate(x => x.Id == contract.Id && !x.IsDelete)
                .Build());

            if (anyContract)
            {
                await _unitOfWork.BeginTransactionAsync();

                var existedContract = await baseContractRepo.GetSingleAsync(new QueryBuilder<Contract>()
                    .WithPredicate(x => x.Id == contract.Id && !x.IsDelete)
                    .Build());
                contract.UpdateNonDefaultProperties(existedContract);
                existedContract.ModifiedBy = user.UserName;
                existedContract.ModifiedDate = DateTime.Now;
                await baseContractRepo.UpdateAsync(existedContract);

                var newSnapshot = new SnapshotMetadata
                {
                    Category = SnapshotMetadataType.Contract.ToString(),
                    CreatedDate = existedContract.ModifiedDate,
                    CreatedBy = existedContract.ModifiedBy,
                    Action = SnapshotMetadataAction.Update.ToString(),
                    IsDelete = false
                };
                newSnapshot.Name =
                    $"{existedContract.Id}_{newSnapshot.Action}_{newSnapshot.CreatedBy}_{newSnapshot.CreatedDate}";
                newSnapshot.StoredData = JsonSerializer.Serialize(existedContract);
                await baseSnapshotRepo.CreateAsync(newSnapshot);

                var saver = await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                if (!saver) return new BaseResponse { IsSuccess = false, Message = Constants.SaveDataFailed };
                return new BaseResponse { IsSuccess = true, Message = Constants.SaveDataSuccess };
            }

            return new BaseResponse { IsSuccess = false, Message = "Hợp đồng không tồn tại." };
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
    }

    /// <summary>
    /// This is used to get contracts that manager can access
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
                      && cda.CreatedDate <= DateTime.Now
                      && (cda.EndDate == null || cda.EndDate >= DateTime.Now)
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

        if (dto.ExpiredDayLeft.HasValue)
        {
            contracts = contracts.Where(x => (x.ExpirationDate - DateTime.Now).Days == dto.ExpiredDayLeft.Value);
        }

        if (dto.IsExpired)
        {
            contracts = contracts.Where(x => x.ExpirationDate < DateTime.Now);
        }

        contracts = contracts.ApplyDateRangeFilter(dto.SignedDate, x => x.SignedDate);
        contracts = contracts.ApplyDateRangeFilter(dto.EffectiveDate, x => x.EffectiveDate);
        contracts = contracts.ApplyDateRangeFilter(dto.ExpirationDate, x => x.ExpirationDate);

        dto.TotalRecord = await contracts.CountAsync();
        var result = await contracts.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return result;
    }


    /// <summary>
    /// This is used to get contracts that employee can access
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    public async Task<List<Contract>> GetContractsByEmployee(ContractGetListDTO dto, string employeeId)
    {
        var baseEmployeeRepo = _unitOfWork.GetRepository<Employee>();
        var employeeDbSet = baseEmployeeRepo.GetDbSet();
        var baseEmpContractRepo = _unitOfWork.GetRepository<EmpContract>();
        var empContractDbSet = baseEmpContractRepo.GetDbSet();
        var baseContractRepo = _unitOfWork.GetRepository<Contract>();
        var contractDbSet = baseContractRepo.GetDbSet();
        var baseContractAssignRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();
        var contractAssignDbSet = baseContractAssignRepo.GetDbSet();

        var contracts = (from e in employeeDbSet
                join cda in contractAssignDbSet on e.DepartmentId equals cda.DepartmentId
                join ec in empContractDbSet on e.Id equals ec.EmployeeId
                join c in contractDbSet on ec.ContractId equals c.Id
                where e.Id == employeeId
                      && !e.IsDelete
                      && !ec.IsDelete
                      && !c.IsDelete
                      && cda.CreatedDate <= DateTime.Now
                      && (cda.EndDate == null || cda.EndDate >= DateTime.Now)
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

        if (dto.ExpiredDayLeft.HasValue)
        {
            contracts = contracts.Where(x => (x.ExpirationDate - DateTime.Now).Days == dto.ExpiredDayLeft.Value);
        }

        if (dto.IsExpired)
        {
            contracts = contracts.Where(x => x.ExpirationDate < DateTime.Now);
        }

        contracts = contracts.ApplyDateRangeFilter(dto.SignedDate, x => x.SignedDate);
        contracts = contracts.ApplyDateRangeFilter(dto.EffectiveDate, x => x.EffectiveDate);
        contracts = contracts.ApplyDateRangeFilter(dto.ExpirationDate, x => x.ExpirationDate);

        dto.TotalRecord = await contracts.CountAsync();
        var result = await contracts.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return result;
    }

    public async Task<Contract> GetContract(long id)
    {
        var baseRepo = _unitOfWork.GetRepository<Contract>();
        var contract = await baseRepo.GetSingleAsync(new QueryBuilder<Contract>()
            .WithPredicate(x => x.Id == id && !x.IsDelete)
            .Build());
        return contract;
    }

    /// <summary>
    /// Check manager has access to contract
    /// </summary>
    /// <returns></returns>
    public async Task<bool> HasManagerAccessToContract(string managerId, long contractId)
    {
        var baseManagerRepo = _unitOfWork.GetRepository<Manager>();
        var baseAssignRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();
        var baseContractRepo = _unitOfWork.GetRepository<Contract>();
        var managerDbSet = baseManagerRepo.GetDbSet();
        var assignDbSet = baseAssignRepo.GetDbSet();
        var contractDbSet = baseContractRepo.GetDbSet();

        var now = DateTime.Now;

        var hasAccess = await (from m in managerDbSet
                join cda in assignDbSet on m.DepartmentId equals cda.DepartmentId
                join c in contractDbSet on cda.ContractId equals c.Id
                where m.Id == managerId
                      && c.Id == contractId
                      && !m.IsDelete
                      && !cda.IsDelete
                      && !c.IsDelete
                      && cda.CreatedDate <= now
                      && (cda.EndDate == null || cda.EndDate >= now)
                select c)
            .AnyAsync();
        return hasAccess;
    }

    /// <summary>
    /// Check employee has access to contract
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="contractId"></param>
    /// <returns></returns>
    public async Task<bool> HasEmployeeAccessToContract(string employeeId, long contractId)
    {
        var baseEmployeeRepo = _unitOfWork.GetRepository<Employee>();
        var employeeDbSet = baseEmployeeRepo.GetDbSet();
        var baseEmpContractRepo = _unitOfWork.GetRepository<EmpContract>();
        var empContractDbSet = baseEmpContractRepo.GetDbSet();
        var baseContractRepo = _unitOfWork.GetRepository<Contract>();
        var contractDbSet = baseContractRepo.GetDbSet();
        var contractAssignBaseRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();
        var contractAssignDbSet = contractAssignBaseRepo.GetDbSet();

        var now = DateTime.Now;

        var hasAccess = await (from e in employeeDbSet
                join cda in contractAssignDbSet on e.DepartmentId equals cda.DepartmentId
                join ec in empContractDbSet on e.Id equals ec.EmployeeId
                join c in contractDbSet on ec.ContractId equals c.Id
                where e.Id == employeeId
                      && c.Id == contractId
                      && !e.IsDelete
                      && !ec.IsDelete
                      && !c.IsDelete
                      && cda.CreatedDate <= now
                      && (cda.EndDate == null || cda.EndDate >= now)
                select c)
            .AnyAsync();
        return hasAccess;
    }

    public async Task<List<Contract>> GetContractsByAdmin(ContractGetListDTO dto)
    {
        var baseRepo = _unitOfWork.GetRepository<Contract>();
        var contracts = baseRepo.Get(new QueryBuilder<Contract>()
            .WithPredicate(x => !x.IsDelete)
            .Build());

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

        if (dto.ExpiredDayLeft.HasValue)
        {
            contracts = contracts.Where(x => (x.ExpirationDate - DateTime.Now).Days == dto.ExpiredDayLeft.Value);
        }

        if (dto.IsExpired)
        {
            contracts = contracts.Where(x => x.ExpirationDate < DateTime.Now);
        }

        contracts = contracts.ApplyDateRangeFilter(dto.SignedDate, x => x.SignedDate);
        contracts = contracts.ApplyDateRangeFilter(dto.EffectiveDate, x => x.EffectiveDate);
        contracts = contracts.ApplyDateRangeFilter(dto.ExpirationDate, x => x.ExpirationDate);

        dto.TotalRecord = await contracts.CountAsync();
        var result = await contracts.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return result;
    }

    /// <summary>
    /// This is used to get a list of departmentId that manage a specific contract
    /// </summary>
    /// <param name="contractId"></param>
    /// <returns></returns>
    public async Task<List<long>> IsContractManagedByWhichDepartment(long contractId)
    {
        var baseContractAssignRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();
        var listDepartmentId = await baseContractAssignRepo.Get(new QueryBuilder<ContractDepartmentAssign>()
                .WithPredicate(x => x.ContractId == contractId
                                                        && x.CreatedDate <= DateTime.Now
                                                        && (x.EndDate == null || x.EndDate >= DateTime.Now)
                                                        && !x.IsDelete)
                .Build())
            .Select(x => x.DepartmentId)
            .ToListAsync();
        return listDepartmentId;
    }

    public async Task<BaseResponse> CreateUpdateContractDepartmentAssign(ContractDepartmentAssign contractDepartmentAssign, ApplicationUser user)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var baseContractRepo = _unitOfWork.GetRepository<Contract>();
            var baseContractAssignRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();
            var baseDepartmentRepo = _unitOfWork.GetRepository<Department>();
            var baseSnapshotRepo = _unitOfWork.GetRepository<SnapshotMetadata>();
            var baseEmployeeRepo = _unitOfWork.GetRepository<Employee>();
            var baseEmpContractRepo = _unitOfWork.GetRepository<EmpContract>();
            
            var anyContract = await baseContractRepo.AnyAsync(new QueryBuilder<Contract>()
                .WithPredicate(x => x.Id == contractDepartmentAssign.ContractId && !x.IsDelete)
                .Build());
            if (!anyContract) return new BaseResponse { IsSuccess = false, Message = "Hợp đồng không tồn tại" };

            var anyDepartment = await baseDepartmentRepo.AnyAsync(new QueryBuilder<Department>()
                .WithPredicate(x => x.Id == contractDepartmentAssign.DepartmentId
                                    && !x.IsDelete)
                .Build());
            if(!anyDepartment) return new BaseResponse { IsSuccess = false, Message = "Phòng ban không tồn tại" };
            
            var anyAssign = await baseContractAssignRepo.AnyAsync(new QueryBuilder<ContractDepartmentAssign>()
                .WithPredicate(x => x.ContractId == contractDepartmentAssign.ContractId
                                    && x.DepartmentId == contractDepartmentAssign.DepartmentId)
                .Build());
            if (anyAssign)
            {
                var existedAssign = await baseContractAssignRepo.GetSingleAsync(
                    new QueryBuilder<ContractDepartmentAssign>()
                        .WithPredicate(x =>
                            x.ContractId == contractDepartmentAssign.ContractId &&
                            x.DepartmentId == contractDepartmentAssign.DepartmentId && !x.IsDelete)
                        .Build());
                existedAssign.EndDate = contractDepartmentAssign.EndDate;
                existedAssign.ModifiedDate = DateTime.Now;
                existedAssign.ModifiedBy = user.UserName;
                await baseContractAssignRepo.UpdateAsync(existedAssign);
                
                var newUpdateSnapshot = new SnapshotMetadata
                {
                    Category = SnapshotMetadataType.Contract.ToString(),
                    CreatedDate = existedAssign.ModifiedDate,
                    CreatedBy = user.UserName,
                    Action = SnapshotMetadataAction.UpdateAssignDeapartment.ToString(),
                    IsDelete = false
                };
            
                newUpdateSnapshot.Name =
                    $"{existedAssign.ContractId}_{existedAssign.DepartmentId}_{newUpdateSnapshot.Action}_{newUpdateSnapshot.CreatedBy}_{newUpdateSnapshot.CreatedDate}";
                newUpdateSnapshot.StoredData = JsonSerializer.Serialize(existedAssign);
                await baseSnapshotRepo.CreateAsync(newUpdateSnapshot);
                
            }
            else
            {
                var newAssign = new ContractDepartmentAssign
                {
                    ContractId = contractDepartmentAssign.ContractId,
                    DepartmentId = contractDepartmentAssign.DepartmentId,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.UserName,
                    EndDate = contractDepartmentAssign.EndDate,
                    IsDelete = false
                };
                await baseContractAssignRepo.CreateAsync(newAssign);
            
                var newSnapshot = new SnapshotMetadata
                {
                    Category = SnapshotMetadataType.Contract.ToString(),
                    CreatedDate = newAssign.CreatedDate,
                    CreatedBy = newAssign.CreatedBy,
                    Action = SnapshotMetadataAction.UpdateAssignDeapartment.ToString(),
                    IsDelete = false
                };
            
                newSnapshot.Name =
                    $"{newAssign.ContractId}_{newAssign.DepartmentId}_{newSnapshot.Action}_{newSnapshot.CreatedBy}_{newSnapshot.CreatedDate}";
                newSnapshot.StoredData = JsonSerializer.Serialize(newAssign);
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

    /// <summary>
    /// This is used to get contracts assign by manager
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="managerId"></param>
    /// <returns></returns>
    public async Task<List<ContractDepartmentAssign>> GetContractDepartmentAssignsByManager(ContractDepartmentAssignGetListDTO dto, string managerId)
    {
        var baseContractAssignRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();
        var baseManagerRepo = _unitOfWork.GetRepository<Manager>();
        var contractAssignDbSet = baseContractAssignRepo.GetDbSet();
        var managerDbSet = baseManagerRepo.GetDbSet();

        var assigns = (from m in managerDbSet
                join cda in contractAssignDbSet on m.DepartmentId equals cda.DepartmentId
                where m.Id == managerId
                      && !m.IsDelete
                      && !cda.IsDelete
                select cda)
            .AsNoTracking()
            .Distinct();

        if (dto.OrderDate.HasValue)
        {
            assigns = assigns.ApplyOrderDate(dto.OrderDate);
        }

        if (dto.ExpirationDaysLeft.HasValue)
        {
            assigns = assigns.Where(x => (x.EndDate.Value - DateTime.Now).Days == dto.ExpirationDaysLeft.Value);
        }
        
        if (dto.IsExpried)
        {
            assigns = assigns.Where(x => x.EndDate.Value < DateTime.Now);
        }
        
        assigns = assigns.ApplyDateRangeFilter(dto.CreatedDate, x => x.CreatedDate);
        assigns = assigns.ApplyDateRangeFilter(dto.EndDate, x => x.EndDate);

        dto.TotalRecord = await assigns.CountAsync();
        var result = await assigns.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return result;
    }
    
    /// <summary>
    /// This is used to get contract department assign for admin
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<List<ContractDepartmentAssign>> GetContractDepartmentAssignsByAdmin(ContractDepartmentAssignGetListDTO dto)
    {
        var baseContractAssignRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();
        var assigns = baseContractAssignRepo.Get(new QueryBuilder<ContractDepartmentAssign>()
            .WithPredicate(x => !x.IsDelete)
            .Build());

        if (dto.OrderDate.HasValue)
        {
            assigns = assigns.ApplyOrderDate(dto.OrderDate);
        }

        if (dto.ExpirationDaysLeft.HasValue)
        {
            assigns = assigns.Where(x => (x.EndDate.Value - DateTime.Now).Days == dto.ExpirationDaysLeft.Value);
        }
        
        if (dto.IsExpried)
        {
            assigns = assigns.Where(x => x.EndDate.Value < DateTime.Now);
        }
        
        assigns = assigns.ApplyDateRangeFilter(dto.CreatedDate, x => x.CreatedDate);
        assigns = assigns.ApplyDateRangeFilter(dto.EndDate, x => x.EndDate);

        dto.TotalRecord = await assigns.CountAsync();
        var result = await assigns.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return result;
    }

    /// <summary>
    /// check manager has access to get contract deparment assign
    /// </summary>
    /// <param name="contractId"></param>
    /// <param name="departmentId"></param>
    /// <param name="managerId"></param>
    /// <returns></returns>
    public async Task<bool> HasManagerAccessToContractDepartmetnAssign(long contractId, long departmentId, string managerId)
    {
        var baseContractAssignRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();
        var baseManagerRepo = _unitOfWork.GetRepository<Manager>();
        var contractAssignDbSet = baseContractAssignRepo.GetDbSet();
        var managerDbSet = baseManagerRepo.GetDbSet();

        var hasAccess = await (from cda in contractAssignDbSet
            join m in managerDbSet on cda.DepartmentId equals m.DepartmentId
            where m.Id == managerId
                  && cda.ContractId == contractId
                  && cda.DepartmentId == departmentId
                  && !m.IsDelete
                  && !cda.IsDelete
            select cda).AnyAsync();
        return false;
    }

    public async Task<ContractDepartmentAssign> GetContractDepartmentAssign(long contractId, long departmentId)
    {
        var baseContractDepartmentRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();
        var contractAssign = await baseContractDepartmentRepo.GetSingleAsync(
            new QueryBuilder<ContractDepartmentAssign>()
                .WithPredicate(x => x.ContractId == contractId && x.DepartmentId == departmentId)
                .Build());
        return contractAssign;
    }
    
    
    public async Task<BaseResponse> CreateUpdateEmpContract(EmpContract empContract, ApplicationUser user)
    {
        var baseEmpContractRepo = _unitOfWork.GetRepository<EmpContract>();
        var baseEmployeeRepo = _unitOfWork.GetRepository<Employee>();
        var baseContractRepo = _unitOfWork.GetRepository<Contract>();
        var baseSnapshotRepo = _unitOfWork.GetRepository<SnapshotMetadata>();

        var anyEmployee = await baseEmployeeRepo.AnyAsync(new QueryBuilder<Employee>()
            .WithPredicate(x => x.Id == empContract.EmployeeId && !x.IsDelete)
            .Build());

        if (!anyEmployee) return new BaseResponse { IsSuccess = false, Message = "Nhân viên không tồn tại." };

        var anyContract = await baseContractRepo.AnyAsync(new QueryBuilder<Contract>()
            .WithPredicate(x => x.Id == empContract.ContractId && !x.IsDelete)
            .Build());
        if (!anyContract) return new BaseResponse { IsSuccess = false, Message = "Hợp đồng không tồn tại" };

        var anyEmpContract = await baseEmpContractRepo.AnyAsync(new QueryBuilder<EmpContract>()
            .WithPredicate(x => x.EmployeeId == empContract.EmployeeId
                                && x.ContractId == empContract.ContractId)
            .Build());

        if (anyEmpContract) return new BaseResponse {IsSuccess = false, Message = "Nhân viên đã được phân công cho hợp đồng."};

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var newEmpContract = new EmpContract
            {
                EmployeeId = empContract.EmployeeId,
                ContractId = empContract.ContractId,
                CreatedDate = DateTime.Now,
                CreatedBy = user.UserName,
                IsDelete = false,
            };
            await baseEmpContractRepo.CreateAsync(newEmpContract);
        
            var newSnapshot = new SnapshotMetadata
            {
                Category = SnapshotMetadataType.Contract.ToString(),
                CreatedDate = newEmpContract.CreatedDate,
                CreatedBy = newEmpContract.CreatedBy,
                Action = SnapshotMetadataAction.AssignEmployee.ToString(),
                IsDelete = false
            };
            
            newSnapshot.Name =
                $"{newEmpContract.ContractId}_{newEmpContract.EmployeeId}_{newSnapshot.Action}_{newSnapshot.CreatedBy}_{newSnapshot.CreatedDate}";
            newSnapshot.StoredData = JsonSerializer.Serialize(newEmpContract);
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
    /// This is used to get contract assigns to employee for Employee
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    public async Task<List<EmpContract>> GetEmpContractsByEmployee(EmpContractGetListDTO dto, string employeeId)
    {
        var baseContractAssignRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();
        var contractAssignDbSet = baseContractAssignRepo.GetDbSet();
        var baseEmpContractRepo = _unitOfWork.GetRepository<EmpContract>();
        var baseEmployeeRepo = _unitOfWork.GetRepository<Employee>();
        var empContractDbSet = baseEmpContractRepo.GetDbSet();
        var employeeDbSet = baseEmployeeRepo.GetDbSet();
        
        var empContracts = (from ec in empContractDbSet
                join e in employeeDbSet on ec.EmployeeId equals e.Id
                join cda in contractAssignDbSet on ec.ContractId equals cda.ContractId
                where e.Id == employeeId
                      && !ec.IsDelete
                      && !e.IsDelete
                      && !cda.IsDelete
                      && cda.DepartmentId == e.DepartmentId
                      && cda.CreatedDate <= DateTime.Now
                      && (cda.EndDate == null || cda.EndDate >= DateTime.Now)
                select ec)
            .AsNoTracking()
            .Distinct();

        if (dto.OrderDate.HasValue)
        {
            empContracts = empContracts.ApplyOrderDate(dto.OrderDate);
        }

        if (!string.IsNullOrEmpty(dto.EmployeeId))
        {
            empContracts = empContracts.Where(x => x.EmployeeId == dto.EmployeeId);
        }

        if (dto.ContractId.HasValue)
        {
            empContracts = empContracts.Where(x => x.ContractId == dto.ContractId);

        }

        empContracts = empContracts.ApplyDateRangeFilter(dto.CreatedDate, x => x.CreatedDate);

        dto.TotalRecord = await empContracts.CountAsync();
        var result = await empContracts.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return result;
    }

    /// <summary>
    /// This is used to get contract employee assigns by mananger
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="managerId"></param>
    /// <returns></returns>
    public async Task<List<EmpContract>> GetEmpContractsByManager(EmpContractGetListDTO dto, string managerId)
    {
        var baseManagerRepo = _unitOfWork.GetRepository<Manager>();
        var managerDbSet = baseManagerRepo.GetDbSet();
        var baseContractAssignRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();
        var contractAssignDbSet = baseContractAssignRepo.GetDbSet();
        var baseEmpContractRepo = _unitOfWork.GetRepository<EmpContract>();
        var empContractDbSet = baseEmpContractRepo.GetDbSet();
        var baseEmployeeRepo = _unitOfWork.GetRepository<Employee>();
        var employeeDbSet = baseEmployeeRepo.GetDbSet();

        var empContracts = (from ec in empContractDbSet
                join e in employeeDbSet on ec.EmployeeId equals e.Id
                join cda in contractAssignDbSet on ec.ContractId equals cda.ContractId
                join m in managerDbSet on cda.DepartmentId equals m.DepartmentId
                where m.Id == managerId
                      && !ec.IsDelete
                      && !cda.IsDelete
                      && !m.IsDelete
                      && !e.IsDelete
                      && e.DepartmentId == m.DepartmentId
                      && cda.CreatedDate <= DateTime.Now
                      && (cda.EndDate == null || cda.EndDate >= DateTime.Now)
                select ec)
            .AsNoTracking()
            .Distinct();
        if (dto.OrderDate.HasValue)
        {
            empContracts = empContracts.ApplyOrderDate(dto.OrderDate);
        }

        if (!string.IsNullOrEmpty(dto.EmployeeId))
        {
            empContracts = empContracts.Where(x => x.EmployeeId == dto.EmployeeId);
        }

        if (dto.ContractId.HasValue)
        {
            empContracts = empContracts.Where(x => x.ContractId == dto.ContractId);

        }

        empContracts = empContracts.ApplyDateRangeFilter(dto.CreatedDate, x => x.CreatedDate);

        dto.TotalRecord = await empContracts.CountAsync();
        var result = await empContracts.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return result;
    }

    public async Task<EmpContract> GetEmpContract(string employeeId, long contractId)
    {
        var baseEmpContractRepo = _unitOfWork.GetRepository<EmpContract>();
        var empContract = await baseEmpContractRepo.GetSingleAsync(new QueryBuilder<EmpContract>()
            .WithPredicate(x => x.EmployeeId == employeeId && x.ContractId == contractId)
            .Build());
        return empContract;
    }

    /// <summary>
    /// This is used to check whether employee has access to get emp contract
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="contractId"></param>
    /// <param name="loggedEmp"></param>
    /// <returns></returns>
    public async Task<bool> HasEmployeeAccessToEmpContract(string employeeId, long contractId, string loggedEmp)
    {
        var baseContractAssignRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();
        var contractAssignDbSet = baseContractAssignRepo.GetDbSet();
        var baseEmpContractRepo = _unitOfWork.GetRepository<EmpContract>();
        var baseEmployeeRepo = _unitOfWork.GetRepository<Employee>();
        var empContractDbSet = baseEmpContractRepo.GetDbSet();
        var employeeDbSet = baseEmployeeRepo.GetDbSet();
        
        var hasAccess = await (from ec in empContractDbSet
                join e in employeeDbSet on ec.EmployeeId equals e.Id
                join cda in contractAssignDbSet on ec.ContractId equals cda.ContractId
                where e.Id == loggedEmp
                      && !ec.IsDelete
                      && ec.EmployeeId == employeeId
                      && ec.ContractId == contractId
                      && !e.IsDelete
                      && !cda.IsDelete
                      && cda.DepartmentId == e.DepartmentId
                select ec).AnyAsync();

        return hasAccess;
    }

    
    /// <summary>
    /// This is used to check whether manager has access to emp contract
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="contractId"></param>
    /// <param name="managerId"></param>
    /// <returns></returns>
    public async Task<bool> HasManagerAccessToEmpContract(string employeeId, long contractId, string managerId)
    {
        var baseContractAssignRepo = _unitOfWork.GetRepository<ContractDepartmentAssign>();
        var baseEmpContractRepo = _unitOfWork.GetRepository<EmpContract>();
        var baseEmployeeRepo = _unitOfWork.GetRepository<Employee>();
        var baseManagerRepo = _unitOfWork.GetRepository<Manager>();
        var contractAssignDbSet = baseContractAssignRepo.GetDbSet();
        var empContractDbSet = baseEmpContractRepo.GetDbSet();
        var employeeDbSet = baseEmployeeRepo.GetDbSet();
        var managerDbSet = baseManagerRepo.GetDbSet();
        
        var hasAccess = await (from ec in empContractDbSet
                join e in employeeDbSet on ec.EmployeeId equals e.Id
                join cda in contractAssignDbSet on ec.ContractId equals cda.ContractId
                join m in managerDbSet on cda.DepartmentId equals m.DepartmentId
                where m.Id == managerId
                      && !ec.IsDelete
                      && !cda.IsDelete
                      && !m.IsDelete
                      && !e.IsDelete
                      && e.DepartmentId == m.DepartmentId
                select ec).AnyAsync();

        return hasAccess;
    }
}