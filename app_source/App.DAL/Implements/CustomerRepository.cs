using App.DAL.Interfaces;
using App.Entity.Entities;
using FS.Commons;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;
using Microsoft.AspNetCore.Http.Extensions;

namespace App.DAL.Implements;

public class CustomerRepository : ICustomerRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public CustomerRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<BaseResponse> CreateUpdateCustomer(Customer customer, long userId)
    {
        var baseCustomerRepo = _unitOfWork.GetRepository<Customer>();
        var any = await baseCustomerRepo.AnyAsync(new QueryBuilder<Customer>()
            .WithPredicate(x => x.Id == customer.Id && x.IsDelete == false)
            .Build());

        if (any)
        {
            var existedCustomer = await baseCustomerRepo.GetSingleAsync(new QueryBuilder<Customer>()
                .WithPredicate(x => x.Id == customer.Id && customer.IsDelete == false)
                .Build());
            if (existedCustomer == null)
                return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy thông tin khách hàng" };
            existedCustomer.Address = customer.Address;
            existedCustomer.TaxIdentificationNumber = customer.TaxIdentificationNumber;
            existedCustomer.CompanyName = customer.CompanyName;
            existedCustomer.Email = customer.Email;
            existedCustomer.PhoneNumber = customer.PhoneNumber;
            existedCustomer.ModifiedDate = DateTime.Now;
            existedCustomer.ModifiedBy = userId.ToString();

            await baseCustomerRepo.UpdateAsync(existedCustomer);
        }
        else
        {
            var newCustomer = new Customer
            {
                CompanyName = customer.CompanyName,
                TaxIdentificationNumber = customer.TaxIdentificationNumber,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                CreatedDate = DateTime.Now,
                CreatedBy = userId.ToString(),
                IsDelete = false
            };
            await baseCustomerRepo.CreateAsync(newCustomer);
        }

        var saver = await _unitOfWork.SaveAsync();
        if (!saver) return new BaseResponse { IsSuccess = false, Message = "Thêm khách hàng không thành công." };
        
        return new BaseResponse { IsSuccess = true, Message = Constants.SaveDataSuccess };
    }
}