using App.DAL.Interfaces;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;
using Microsoft.Extensions.Logging;

namespace App.DAL.Implements;

public class ManagerRepository : IManagerRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;
    private readonly ILogger<ManagerRepository> _logger;

    public ManagerRepository(IFSUnitOfWork<AppDbContext> unitOfWork, ILogger<ManagerRepository> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }


    public async Task<BaseResponse> CreateUpdateManager(Manager manager, ApplicationUser user)
    {
        try
        {
            var repoBase = _unitOfWork.GetRepository<Manager>();
            await _unitOfWork.BeginTransactionAsync();
            var any = await repoBase.AnyAsync(new QueryBuilder<Manager>()
                .WithPredicate(x =>  x.Id.Equals(manager.Id) && x.DepartmentId == manager.DepartmentId)
                .Build());
            if (any)
            {
                _logger.LogError("Da vao trong UPDATE");
                var existedManager = await repoBase.GetSingleAsync(new QueryBuilder<Manager>()
                    .WithPredicate(x => x.Id.Equals(manager.Id)
                                        && x.DepartmentId == manager.DepartmentId
                                        && x.IsDelete == false)
                    .Build());
                if (existedManager == null)
                    return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy quản lý." };
                if (!existedManager.CreatedBy.Equals($"{user.FirstName} {user.LastName}"))
                    return new BaseResponse { IsSuccess = false, Message = Constants.UserNotSame };
                existedManager.DepartmentId = manager.DepartmentId;
                existedManager.ModifiedDate = DateTime.Now;
                existedManager.ModifiedBy = $"{user.FirstName} {user.LastName}";

                await repoBase.UpdateAsync(existedManager);
            }
            else
            {
                var newManager = new Manager
                {
                    Id = manager.Id,
                    DepartmentId = manager.DepartmentId,
                    CreatedDate = DateTime.Now,
                    CreatedBy = $"{user.FirstName} {user.LastName}"
                };
                await repoBase.CreateAsync(newManager);
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

    public async Task<Manager> GetManager(long userId)
    {
        var baseRepo = _unitOfWork.GetRepository<Manager>();
        var manager = await baseRepo.GetSingleAsync(new QueryBuilder<Manager>()
            .WithPredicate(x => x.Id.Equals(userId.ToString()) && x.IsDelete == false)
            .Build());
        return manager;
    }

    /// <summary>
    /// This is used to get manager of a department
    /// </summary>
    /// <param name="departmentId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Manager> GetManagerByDepartmentId(long departmentId)
    {
        var baseRepo = _unitOfWork.GetRepository<Manager>();
        var manager = await baseRepo.GetSingleAsync(new QueryBuilder<Manager>()
            .WithPredicate(x => x.DepartmentId.Equals(departmentId) && x.IsDelete == false)
            .Build());
        return manager;
    }
}