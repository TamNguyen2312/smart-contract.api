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
            .WithPredicate(x => x.IsDelete == false && x.CreatedBy.Equals(user.UserName))
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

    /// <summary>
    /// method check permission of manager when access customer information
    /// </summary>
    /// <param name="managerId"></param>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public async Task<bool> ManagerHasAccessToCustomerAsync(string managerId, long customerId)
    {
        var managerBaseRepo = _unitOfWork.GetRepository<Manager>();
        var assignBaseRepo = _unitOfWork.GetRepository<CustomerDepartmentAssign>();
        var managerDbSet = managerBaseRepo.GetDbSet();
        var assignDbSet = assignBaseRepo.GetDbSet();

        var hasAccess = await (from cda in assignDbSet
            join m in managerDbSet on cda.DeparmentId equals m.DepartmentId
            where cda.CustomerId == customerId
                  && m.Id == managerId
                  && !cda.IsDelete
                  && !m.IsDelete
            select cda).AnyAsync();

        return hasAccess;
    }

    /// <summary>
    /// method get all customer that manager can access
    /// </summary>
    /// <param name="managerId"></param>
    /// <returns></returns>
    public async Task<List<Customer>> GetCustomersByManagerAsync(CustomerGetListDTO dto, string managerId)
    {
        var managerBaseRepo = _unitOfWork.GetRepository<Manager>();
        var assignBaseRepo = _unitOfWork.GetRepository<CustomerDepartmentAssign>();
        var customerBaseRepo = _unitOfWork.GetRepository<Customer>();
        var managerDbSet = managerBaseRepo.GetDbSet();
        var assignDbSet = assignBaseRepo.GetDbSet();
        var customerDbSet = customerBaseRepo.GetDbSet();

        var customers = (from m in managerDbSet
                join cda in assignDbSet on m.DepartmentId equals cda.DeparmentId
                join c in customerDbSet on cda.CustomerId equals c.Id
                where m.Id == managerId
                      && !m.IsDelete
                      && !cda.IsDelete
                      && !c.IsDelete
                select c)
            .AsNoTracking()
            .Distinct();

        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            customers = customers.Where(x => x.CompanyName.Contains(dto.Keyword)
                                             || x.Address.Contains(dto.Keyword)
                                             || x.Email.Contains(dto.Keyword)
                                             || x.PhoneNumber.Contains(dto.Keyword));
        }

        if (dto.OrderDate.HasValue)
        {
            customers = customers.ApplyOrderDate(dto.OrderDate);
        }

        dto.TotalRecord = await customers.CountAsync();

        var result = await customers.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();

        return result;
    }


    /// <summary>
    /// This is used to get dropdown list customer by Admin using department id
    /// </summary>
    /// <param name="departmentId"></param>
    /// <returns></returns>
    public async Task<List<Customer>> GetDropdownCustomerByAdmin(long departmentId)
    {
        var baseCustomerAssignRepo = _unitOfWork.GetRepository<CustomerDepartmentAssign>();
        var baseCustomerRepo = _unitOfWork.GetRepository<Customer>();
        var customerAssignDbSet = baseCustomerAssignRepo.GetDbSet();
        var customerDbSet = baseCustomerRepo.GetDbSet();

        var customers = await (from cda in customerAssignDbSet
            join c in customerDbSet on cda.CustomerId equals c.Id
            where cda.DeparmentId == departmentId
                  && !cda.IsDelete
                  && !c.IsDelete
            select new Customer
            {
                Id = c.Id,
                CompanyName = c.CompanyName
            }).AsNoTracking().Distinct().ToListAsync();
        return customers;
    }

    /// <summary>
    /// This is used to get dropdown list customer by Manager or Employee using department id
    /// </summary>
    /// <param name="departmentId"></param>
    /// <param name="id"></param>
    /// <param name="isManager"></param>
    /// <returns></returns>
    public async Task<List<Customer>> GetDropdownCustomerByManagerOrEmployee(string id,
        bool isManager)
    {
        var baseCustomerAssignRepo = _unitOfWork.GetRepository<CustomerDepartmentAssign>();
        var baseCustomerRepo = _unitOfWork.GetRepository<Customer>();
        var customerAssignDbSet = baseCustomerAssignRepo.GetDbSet();
        var customerDbSet = baseCustomerRepo.GetDbSet();

        if (isManager)
        {
            var managerBaseRepo = _unitOfWork.GetRepository<Manager>();
            var managerDbSet = managerBaseRepo.GetDbSet();

            var customers = await (from m in managerDbSet
                    join cda in customerAssignDbSet on m.DepartmentId equals cda.DeparmentId
                    join c in customerDbSet on cda.CustomerId equals c.Id
                    where m.Id == id
                          && !m.IsDelete
                          && !cda.IsDelete
                          && !c.IsDelete
                    select new Customer
                    {
                        Id = c.Id,
                        CompanyName = c.CompanyName
                    })
                .AsNoTracking()
                .Distinct()
                .ToListAsync();
            return customers;
        }
        else
        {
            var employeeBaseRepo = _unitOfWork.GetRepository<Employee>();
            var employeeDbSet = employeeBaseRepo.GetDbSet();

            var customers = await (from e in employeeDbSet
                    join cda in customerAssignDbSet on e.DepartmentId equals cda.DeparmentId
                    join c in customerDbSet on cda.CustomerId equals c.Id
                    where e.Id == id
                          && !e.IsDelete
                          && !cda.IsDelete
                          && !c.IsDelete
                    select new Customer
                    {
                        Id = c.Id,
                        CompanyName = c.CompanyName
                    })
                .AsNoTracking()
                .Distinct()
                .ToListAsync();
            return customers;
        }
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