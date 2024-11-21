using App.DAL.Interfaces;
using App.Entity.DTOs.Customer;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implements;

public class CustomerRepository : ICustomerRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public CustomerRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse> CreateUpdateCustomer(Customer customer, ApplicationUser user)
    {
        var baseCustomerRepo = _unitOfWork.GetRepository<Customer>();
        var any = await baseCustomerRepo.AnyAsync(new QueryBuilder<Customer>()
            .WithPredicate(x => x.Id == customer.Id && x.IsDelete == false)
            .Build());

        if (any)
        {
            var existedCustomer = await baseCustomerRepo.GetSingleAsync(new QueryBuilder<Customer>()
                .WithPredicate(x => x.Id == customer.Id &&
                                    customer.IsDelete == false)
                .Build());
            if (existedCustomer == null)
                return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy thông tin khách hàng" };
            if (!existedCustomer.CreatedBy.Equals(user.UserName))
                return new BaseResponse { IsSuccess = false, Message = Constants.UserNotSame };
            customer.UpdateNonDefaultProperties(existedCustomer);
            existedCustomer.ModifiedDate = DateTime.Now;
            existedCustomer.ModifiedBy = user.UserName;

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
                CreatedBy = user.UserName,
                IsDelete = false
            };
            await baseCustomerRepo.CreateAsync(newCustomer);
        }

        var saver = await _unitOfWork.SaveAsync();
        if (!saver) return new BaseResponse { IsSuccess = false, Message = "Thêm khách hàng không thành công." };

        return new BaseResponse { IsSuccess = true, Message = Constants.SaveDataSuccess };
    }

    public async Task<List<Customer>> GetAllCustomers(CustomerGetListDTO dto, ApplicationUser user)
    {
        var baseCustomerRepo = _unitOfWork.GetRepository<Customer>();
        var loadedRecords = baseCustomerRepo.Get(new QueryBuilder<Customer>()
            .WithPredicate(x => x.IsDelete == false && x.CreatedBy.Equals(user.Email))
            .Build());
        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            loadedRecords = loadedRecords.Where(x =>
                x.PhoneNumber.Contains(dto.Keyword) || x.CompanyName.Contains(dto.Keyword));
        }

        loadedRecords = loadedRecords.ApplyOrderDate(dto.OrderDate);
        
        dto.TotalRecord = await loadedRecords.CountAsync();
        var response = await loadedRecords.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return response;
    }

    public async Task<Customer> GetCustomer(long customerId, ApplicationUser user)
    {
        var baseCustomerRepo = _unitOfWork.GetRepository<Customer>();
        var customer = await baseCustomerRepo.GetSingleAsync(new QueryBuilder<Customer>()
            .WithPredicate(x => x.Id == customerId &&
                                x.CreatedBy.Equals(user.Email)
                                && x.IsDelete == false)
            .Build());
        return customer;
    }
    
    public async Task<Customer> GetCustomer(long customerId)
    {
        var baseCustomerRepo = _unitOfWork.GetRepository<Customer>();
        var customer = await baseCustomerRepo.GetSingleAsync(new QueryBuilder<Customer>()
            .WithPredicate(x => x.Id == customerId && x.IsDelete == false)
            .Build());
        return customer;
    }
    
    public async Task<Customer> GetCustomerByEmail(string email)
    {
        var baseCustomerRepo = _unitOfWork.GetRepository<Customer>();
        if (email.IndexOf('@') >= 0)
        {
            // Find by email:
            var customer = await baseCustomerRepo.GetSingleAsync(new QueryBuilder<Customer>()
                .WithPredicate(x => x.Email.Equals(email)
                                    && x.IsDelete == false)
                .Build());
            if (customer == null)
            {
                customer = await baseCustomerRepo.GetSingleAsync(new QueryBuilder<Customer>()
                    .WithPredicate(x => x.Email.Replace(".", "") == email.Replace(".", "")
                                        && x.IsDelete == false)
                    .Build());
            }
            return customer;
        }
        return null;
    }

    public async Task<BaseResponse> DeleteCustomer(long customerId, ApplicationUser user)
    {
        var baseCustomerRepo = _unitOfWork.GetRepository<Customer>();
        var customer = await baseCustomerRepo.GetSingleAsync(new QueryBuilder<Customer>()
            .WithPredicate(x => x.Id == customerId &&
                                x.CreatedBy.Equals(user.Email)
                                && x.IsDelete == false)
            .Build());
        if (customer == null) return new BaseResponse { IsSuccess = false, Message = Constants.GetNotFound };

        //nên check thêm các ràng buộc sau này
        customer.IsDelete = true;
        await baseCustomerRepo.UpdateAsync(customer);
        var saver = await _unitOfWork.SaveAsync();
        if (!saver) return new BaseResponse { IsSuccess = false, Message = "Xoá khách hàng không thành công." };
        return new BaseResponse { IsSuccess = true, Message = "Xoá khách hàng thành công." };
    }
}