using App.Entity.DTOs.Customer;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface ICustomerRepository
{
    Task<BaseResponse> CreateUpdateCustomer(Customer customer, ApplicationUser user);
    Task<List<Customer>> GetAllCustomers(CustomerGetListDTO dto, ApplicationUser user);
    Task<bool> ManagerHasAccessToCustomerAsync(string managerId, long customerId);
    Task<Customer> GetCustomer(long customerId, ApplicationUser user);
    Task<Customer> GetCustomer(long customerId);
    Task<BaseResponse> DeleteCustomer(long customerId, ApplicationUser user);
    Task<Customer> GetCustomerByEmail(string email);
}