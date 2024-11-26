using App.Entity.DTOs.Customer;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface ICustomerRepository
{
    Task<BaseResponse> CreateUpdateCustomer(Customer customer, ApplicationUser user);
    Task<List<Customer>> GetAllCustomers(CustomerGetListDTO dto, ApplicationUser user);
    Task<List<Customer>> GetCustomersByManagerAsync(CustomerGetListDTO dto, string managerId);
    Task<bool> ManagerHasAccessToCustomerAsync(string managerId, long customerId);
    Task<Customer> GetCustomer(long customerId, ApplicationUser user);
    Task<Customer> GetCustomer(long customerId);
    Task<BaseResponse> DeleteCustomer(long customerId, ApplicationUser user);
    Task<Customer> GetCustomerByEmail(string email);
    Task<List<Customer>> GetDropdownCustomerByAdmin(long departmentId);

    Task<List<Customer>> GetDropdownCustomerByManagerOrEmployee(string id,
        bool isManager);
}