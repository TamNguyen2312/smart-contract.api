using App.DAL.Interfaces;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;

namespace App.DAL.Implements;

public class CustomerDocumentRepository : ICustomerDocumentRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public CustomerDocumentRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<BaseResponse> CreateUpdateCustomerDocument(CustomerDocument customerDocument, ApplicationUser user)
    {
        var baseRepo = _unitOfWork.GetRepository<CustomerDocument>();
        var any = await baseRepo.AnyAsync(new QueryBuilder<CustomerDocument>()
            .WithPredicate(x => x.Id == customerDocument.Id && x.IsDelete == false)
            .Build());

        if (any)
        {
            var existedCustomer = await baseRepo.GetSingleAsync(new QueryBuilder<CustomerDocument>()
                .WithPredicate(x => x.Id == customerDocument.Id &&
                                    customerDocument.IsDelete == false)
                .Build());
            if (existedCustomer == null)
                return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy tài liệu của khách hàng" };
            if (!existedCustomer.CreatedBy.Equals(user.UserName))
                return new BaseResponse { IsSuccess = false, Message = Constants.UserNotSame };
            customerDocument.UpdateNonDefaultProperties(existedCustomer);
            existedCustomer.ModifiedDate = DateTime.Now;
            existedCustomer.ModifiedBy = user.UserName;

            await baseRepo.UpdateAsync(existedCustomer);
        }
        else
        {
            var newCustomerDocument = new CustomerDocument
            {
                Description = customerDocument.Description,
                FilePath = customerDocument.FilePath,
                CreatedDate = DateTime.Now,
                CreatedBy = user.UserName,
                CustomerId = customerDocument.CustomerId,
                IsDelete = false
            };
            await baseRepo.CreateAsync(newCustomerDocument);
        }

        var saver = await _unitOfWork.SaveAsync();
        if (!saver) return new BaseResponse { IsSuccess = false, Message = "Thêm tài liệu của khách hàng không thành công." };

        return new BaseResponse { IsSuccess = true, Message = Constants.SaveDataSuccess };
    }
    
    public async Task<CustomerDocument> GetCustomerDocument(long id)
    {
        var baseRepo = _unitOfWork.GetRepository<CustomerDocument>();
        var customerDocument = await baseRepo.GetSingleAsync(new QueryBuilder<CustomerDocument>()
            .WithPredicate(x => x.Id == id && x.IsDelete == false)
            .Build());
        return customerDocument;
    }
}