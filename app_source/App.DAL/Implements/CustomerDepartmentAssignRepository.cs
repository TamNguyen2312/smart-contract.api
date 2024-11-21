using App.DAL.Interfaces;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;

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
}