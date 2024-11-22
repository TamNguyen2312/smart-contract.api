using App.DAL.Interfaces;
using App.Entity.DTOs.CustomerDocument;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implements;

public class CustomerDocumentRepository : ICustomerDocumentRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public CustomerDocumentRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse> CreateUpdateCustomerDocument(CustomerDocument customerDocument,
        ApplicationUser user)
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
        if (!saver)
            return new BaseResponse { IsSuccess = false, Message = "Thêm tài liệu của khách hàng không thành công." };

        return new BaseResponse { IsSuccess = true, Message = Constants.SaveDataSuccess };
    }


    /// <summary>
    /// This is used to get all customer document by admin
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="userName"></param>
    /// <returns></returns>
    public async Task<List<CustomerDocument>> GetAllCustomerDocuments(CustomerDocumentGetListDTO dto, string userName)
    {
        var baseRepo = _unitOfWork.GetRepository<CustomerDocument>();
        var documents = baseRepo.Get(new QueryBuilder<CustomerDocument>()
            .WithPredicate(x => !x.IsDelete && x.CreatedBy.Equals(userName))
            .Build());

        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            documents = documents.Where(x => x.Description.Contains(dto.Keyword) || x.FilePath.Contains(dto.Keyword));
        }

        if (dto.CustomerId.HasValue)
        {
            documents = documents.Where(x => x.CustomerId == dto.CustomerId.Value);
        }

        if (dto.OrderDate.HasValue)
        {
            documents = documents.ApplyOrderDate(dto.OrderDate);
        }

        dto.TotalRecord = await documents.CountAsync();

        var result = await documents.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return result;
    }


    /// <summary>
    /// This is used to get list of customer document that manager can access
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="managerId"></param>
    /// <returns></returns>
    public async Task<List<CustomerDocument>> GetCustomerDocumentsByManagerAsync(CustomerDocumentGetListDTO dto,
        string managerId)
    {
        var baseManagerRepo = _unitOfWork.GetRepository<Manager>();
        var baseAssignRepo = _unitOfWork.GetRepository<CustomerDepartmentAssign>();
        var baseCustomerDocumentRepo = _unitOfWork.GetRepository<CustomerDocument>();
        var managerDbSet = baseManagerRepo.GetDbSet();
        var assignDbSet = baseAssignRepo.GetDbSet();
        var documentDbSet = baseCustomerDocumentRepo.GetDbSet();
        var customerDocuments = (from m in managerDbSet
                join cda in assignDbSet on m.DepartmentId equals cda.DeparmentId
                join cd in documentDbSet on cda.CustomerId equals cd.CustomerId
                where m.Id == managerId
                      && !m.IsDelete
                      && !cda.IsDelete
                      && !cd.IsDelete
                select cd)
            .AsNoTracking()
            .Distinct();

        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            customerDocuments = customerDocuments.Where(x =>
                x.Description.Contains(dto.Keyword) || x.FilePath.Contains(dto.Keyword));
        }

        if (dto.OrderDate.HasValue)
        {
            customerDocuments = customerDocuments.ApplyOrderDate(dto.OrderDate);
        }

        if (dto.CustomerId.HasValue)
        {
            customerDocuments = customerDocuments.Where(x => x.CustomerId == dto.CustomerId.Value);
        }

        dto.TotalRecord = await customerDocuments.CountAsync();

        var result = await customerDocuments.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return result;
    }
    
    /// <summary>
    /// This is used to check access of logged manager
    /// </summary>
    /// <param name="managerId"></param>
    /// <param name="customerDocumentId"></param>
    /// <returns></returns>
    public async Task<bool> ManagerHasAccessToCustomerDocumentAsync(string managerId, long customerDocumentId)
    {
        var baseManagerRepo = _unitOfWork.GetRepository<Manager>();
        var baseAssignRepo = _unitOfWork.GetRepository<CustomerDepartmentAssign>();
        var baseCustomerDocumentRepo = _unitOfWork.GetRepository<CustomerDocument>();
        var managerDbSet = baseManagerRepo.GetDbSet();
        var assignDbSet = baseAssignRepo.GetDbSet();
        var documentDbSet = baseCustomerDocumentRepo.GetDbSet();
        var hasAccess = await (from m in managerDbSet
            join cda in assignDbSet on m.DepartmentId equals cda.DeparmentId
            join cd in documentDbSet on cda.CustomerId equals cd.CustomerId
            where m.Id == managerId
                  && cd.Id == customerDocumentId
                  && !m.IsDelete
                  && !cda.IsDelete
                  && !cd.IsDelete
            select cd).AnyAsync();

        return hasAccess;
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