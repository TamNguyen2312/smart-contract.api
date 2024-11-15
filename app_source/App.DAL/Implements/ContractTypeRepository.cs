using System;
using App.DAL.Interfaces;
using App.Entity.DTOs.ContractType;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implements;

public class ContractTypeRepository : IContractTypeRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public ContractTypeRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }
    public async Task<BaseResponse> CreateUpdateContractType(ContractType contractType, ApplicationUser user)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var baseRepo = _unitOfWork.GetRepository<ContractType>();

            var any = await baseRepo.AnyAsync(new QueryBuilder<ContractType>()
                                            .WithPredicate(x => x.Id == contractType.Id)
                                            .Build());
            if (any)
            {
                var existed = await baseRepo.GetSingleAsync(new QueryBuilder<ContractType>()
                                                            .WithPredicate(x => x.Id == contractType.Id && x.IsDelete == false)
                                                            .Build());
                if (existed == null) return new BaseResponse { IsSuccess = false, Message = "Loại hợp đồng không tồn tại." };
                contractType.UpdateNonDefaultProperties(existed);
                existed.ModifiedBy = user.Email;
                existed.ModifiedDate = DateTime.Now;
                await baseRepo.UpdateAsync(existed);
            }
            else
            {
                var newContractType = new ContractType
                {
                    Name = contractType.Name,
                    CreatedBy = user.Email,
                    CreatedDate = DateTime.Now
                };
                await baseRepo.CreateAsync(newContractType);
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

    public async Task<List<ContractType>> GetAllContractType(ContractTypeGetListDTO dto)
    {
        var baseRepo = _unitOfWork.GetRepository<ContractType>();
        var loadedRecords = baseRepo.Get(new QueryBuilder<ContractType>()
                                                           .WithPredicate(x => x.IsDelete == false)
                                                           .Build());
        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            loadedRecords = loadedRecords.Where(x => x.Name.Contains(dto.Keyword));
        }

        if (dto.OrderDate.HasValue)
        {
            switch ((int)dto.OrderDate.Value)
            {
                case 1:
                    loadedRecords = loadedRecords.OrderByDescending(x => x.CreatedDate);
                    break;
                case 2:
                    loadedRecords = loadedRecords.OrderBy(x => x.CreatedDate);
                    break;
                case 3:
                    loadedRecords = loadedRecords.OrderByDescending(x => x.ModifiedDate ?? DateTime.MinValue);
                    break;
                case 4:
                    loadedRecords = loadedRecords.OrderBy(x => x.ModifiedDate ?? DateTime.MaxValue);
                    break;
            }
        }
        dto.TotalRecord = await loadedRecords.CountAsync();
        return await loadedRecords.ToPagedList<ContractType>(dto.PageIndex, dto.PageSize).ToListAsync();
    }

    public async Task<ContractType> GetContractTypeById(long id)
    {
        var baseRepo = _unitOfWork.GetRepository<ContractType>();
        var existed = await baseRepo.GetSingleAsync(new QueryBuilder<ContractType>()
                                                           .WithPredicate(x => x.Id == id && x.IsDelete == false)
                                                           .Build());
        return existed;
    }
}
