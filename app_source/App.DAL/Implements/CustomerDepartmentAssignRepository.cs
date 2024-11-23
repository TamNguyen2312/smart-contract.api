using App.DAL.Interfaces;
using App.Entity.DTOs.CustomerDeparmentAssign;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implements;

public class CustomerDepartmentAssignRepository : ICustomerDepartmentAssignRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public CustomerDepartmentAssignRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<BaseResponse> CreateUpdateCusomterDepartmentAssign(CustomerDepartmentAssign assign, ApplicationUser user)
    {
        var baseRepo = _unitOfWork.GetRepository<CustomerDepartmentAssign>();
        var any = await baseRepo.AnyAsync(new QueryBuilder<CustomerDepartmentAssign>()
            .WithPredicate(x => x.Id == assign.Id && x.IsDelete == false)
            .Build());

        if (any)
        {
            var existedCustomer = await baseRepo.GetSingleAsync(new QueryBuilder<CustomerDepartmentAssign>()
                .WithPredicate(x => x.Id == assign.Id &&
                                    assign.IsDelete == false)
                .Build());
            if (existedCustomer == null)
                return new BaseResponse { IsSuccess = false, Message = "Không tìm phân công khách hàng" };
            if (!existedCustomer.CreatedBy.Equals(user.UserName))
                return new BaseResponse { IsSuccess = false, Message = Constants.UserNotSame };
            assign.UpdateNonDefaultProperties(existedCustomer);
            existedCustomer.ModifiedDate = DateTime.Now;
            existedCustomer.ModifiedBy = user.UserName;

            await baseRepo.UpdateAsync(existedCustomer);
        }
        else
        {
            var newAssign = new CustomerDepartmentAssign
            {
                CustomerId = assign.CustomerId,
                DeparmentId = assign.DeparmentId,
                Description = assign.Description,
                CreatedDate = DateTime.Now,
                CreatedBy = user.UserName,
                IsDelete = false
            };
            await baseRepo.CreateAsync(newAssign);
        }

        var saver = await _unitOfWork.SaveAsync();
        if (!saver) return new BaseResponse { IsSuccess = false, Message = "Thêm phân công khách hàng không thành công." };

        return new BaseResponse { IsSuccess = true, Message = Constants.SaveDataSuccess };
    }

    public async Task<List<CustomerDepartmentAssign>> GetCustomerDepartmentAssignsByAdmin(CustomerDepartmentAssignGetListDTO dto, string userName)
    {
        var baseRepo = _unitOfWork.GetRepository<CustomerDepartmentAssign>();
        var query = baseRepo.Get(new QueryBuilder<CustomerDepartmentAssign>()
            .WithPredicate(x => !x.IsDelete && x.CreatedBy.Equals(userName))
            .Build());
        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            query = query.Where(x => x.Description.Contains(dto.Keyword));
        }

        if (dto.OrderDate.HasValue)
        {
            query = query.ApplyOrderDate(dto.OrderDate);
        }

        if (dto.CustomerOrDepartmentId.HasValue)
        {
            query = query.Where(x => x.CustomerId == dto.CustomerOrDepartmentId.Value || x.DeparmentId == dto.CustomerOrDepartmentId.Value);
        }

        dto.TotalRecord = await query.CountAsync();
        var result = await query.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return result;
    }
    
    public async Task<List<CustomerDepartmentAssign>> GetCustomerDepartmentAssignsByManager(CustomerDepartmentAssignGetListDTO dto, string managerId)
    {
        var baseManagerRepo = _unitOfWork.GetRepository<Manager>();
        var baseAssignRepo = _unitOfWork.GetRepository<CustomerDepartmentAssign>();
        var managerDbSet = baseManagerRepo.GetDbSet();
        var assignDbSet = baseAssignRepo.GetDbSet();
        
        var query = (from m in managerDbSet
                join cda in assignDbSet on m.DepartmentId equals cda.DeparmentId
                where m.Id == managerId
                      && !m.IsDelete
                      && !cda.IsDelete
                select cda)
            .AsNoTracking()
            .Distinct();
        
        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            query = query.Where(x => x.Description.Contains(dto.Keyword));
        }

        if (dto.OrderDate.HasValue)
        {
            query = query.ApplyOrderDate(dto.OrderDate);
        }

        if (dto.CustomerOrDepartmentId.HasValue)
        {
            query = query.Where(x => x.CustomerId == dto.CustomerOrDepartmentId.Value || x.DeparmentId == dto.CustomerOrDepartmentId.Value);
        }

        dto.TotalRecord = await query.CountAsync();
        var result = await query.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return result;
    }

    /// <summary>
    /// This is used to check whether customer is assigned in department
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="departmentId"></param>
    /// <returns></returns>
    public async Task<bool> IsCustomerAssignedIn(long customerId, long departmentId)
    {
        var baseRepo = _unitOfWork.GetRepository<CustomerDepartmentAssign>();
        var any = await baseRepo.AnyAsync(new QueryBuilder<CustomerDepartmentAssign>()
            .WithPredicate(x => x.CustomerId == customerId && x.DeparmentId == departmentId)
            .Build());
        if (!any) return false;
        return true;
    }
    
    
    /// <summary>
    /// Check access of logged manager
    /// </summary>
    /// <param name="managerId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> ManagerHasAccessToAssignAsync(string managerId, long id)
    {
        var baseManagerRepo = _unitOfWork.GetRepository<Manager>();
        var baseAssignRepo = _unitOfWork.GetRepository<CustomerDepartmentAssign>();
        var managerDbSet = baseManagerRepo.GetDbSet();
        var assignDbSet = baseAssignRepo.GetDbSet();
        var hasAccess = await (from m in managerDbSet
            join cda in assignDbSet on m.DepartmentId equals cda.DeparmentId
            where m.Id == managerId
                  && cda.Id == id
                  && !m.IsDelete
                  && !cda.IsDelete
            select cda).AnyAsync();

        return hasAccess;
    }

    /// <summary>
    /// Get assign using its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<CustomerDepartmentAssign> GetCustomerDepartmentAssign(long id)
    {
        var baseRepo = _unitOfWork.GetRepository<CustomerDepartmentAssign>();
        var assign = await baseRepo.GetSingleAsync(new QueryBuilder<CustomerDepartmentAssign>()
            .WithPredicate(x => x.Id == id && !x.IsDelete)
            .Build());
        return assign;
    }
    
    /// <summary>
    /// Get assign but using customer id and departmentId
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="departmentId"></param>
    /// <returns></returns>
    public async Task<CustomerDepartmentAssign> GetCustomerDepartmentAssign(long customerId, long departmentId)
    {
        var baseRepo = _unitOfWork.GetRepository<CustomerDepartmentAssign>();
        var assign = await baseRepo.GetSingleAsync(new QueryBuilder<CustomerDepartmentAssign>()
            .WithPredicate(x => x.CustomerId == customerId && x.DeparmentId == departmentId && !x.IsDelete)
            .Build());
        return assign;
    }
}