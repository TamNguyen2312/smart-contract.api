using App.DAL.Interfaces;
using App.Entity.DTOs.Employee;
using App.Entity.DTOs.Manager;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;
using FS.DAL.Implements;
using FS.DAL.Interfaces;
using FS.DAL.Queries;
using FS.IdentityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.DAL.Implements;

public class ManagerRepository : IManagerRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;
    private readonly ILogger<ManagerRepository> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public ManagerRepository(IFSUnitOfWork<AppDbContext> unitOfWork,
                            ILogger<ManagerRepository> logger,
                            UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _userManager = userManager;
    }


    public async Task<BaseResponse> CreateUpdateManager(Manager manager, ApplicationUser user)
    {
        try
        {
            var repoBase = _unitOfWork.GetRepository<Manager>();
            await _unitOfWork.BeginTransactionAsync();
            var any = await repoBase.AnyAsync(new QueryBuilder<Manager>()
                .WithPredicate(x => x.DepartmentId == manager.DepartmentId)
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
                if (!existedManager.CreatedBy.Equals(user.UserName))
                    return new BaseResponse { IsSuccess = false, Message = Constants.UserNotSame };
                existedManager.DepartmentId = manager.DepartmentId;
                existedManager.ModifiedDate = DateTime.Now;
                existedManager.ModifiedBy = user.UserName;

                await repoBase.UpdateAsync(existedManager);
            }
            else
            {
                var newManager = new Manager
                {
                    Id = manager.Id,
                    DepartmentId = manager.DepartmentId,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.UserName
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

    public async Task<List<UserManagerDTO>> GetAllManager(AccountGetListDTO dto)
    {
        var baseManagerRepo = _unitOfWork.GetRepository<Manager>();
        var managerQuery = baseManagerRepo.Get(new QueryBuilder<Manager>()
            .WithPredicate(x => x.IsDelete == false)
            .Build());

        if (dto.DepartmentId.HasValue)
        {
            managerQuery = managerQuery.Where(x => x.DepartmentId == dto.DepartmentId.Value);
        }
        
        managerQuery = managerQuery.ApplyOrderDate(dto.OrderDate);
        
        var managerIds = await managerQuery.Select(manager => manager.Id).ToListAsync();
        
        var userQuery = _userManager.Users.Where(user => managerIds.Contains(user.Id.ToString()));

        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            userQuery = userQuery.Where(x => x.FirstName.Contains(dto.Keyword)
                                             || x.LastName.Contains(dto.Keyword)
                                             || x.Email.Contains(dto.Keyword)
                                             || x.UserName.Contains(dto.Keyword));
        }
        
        dto.TotalRecord = await userQuery.CountAsync();
        
        var pagedUsers = await userQuery.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        
        var userIds = pagedUsers.Select(u => u.Id).ToList();
        
        var managers = await managerQuery
            .Where(emp => userIds.Contains(Convert.ToInt64(emp.Id)))
            .ToListAsync();
        
        var result = pagedUsers.Join(managers, 
                user => user.Id.ToString(), 
                manager => manager.Id, 
                (user, manager) => new UserManagerDTO
                {
                    User = user,
                    Manager = manager
                })
            .ToList();
        return result;
    }

    /// <summary>
    /// Check that department has a manager ?
    /// </summary>
    /// <param name="departmentId"></param>
    /// <returns></returns>
    public async Task<bool> HasManagerInDepartment(long departmentId)
    {
        var baseRepo = _unitOfWork.GetRepository<Manager>();
        var manager = await baseRepo.AnyAsync(new QueryBuilder<Manager>()
            .WithPredicate(x => x.DepartmentId.Equals(departmentId) && x.IsDelete == false)
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