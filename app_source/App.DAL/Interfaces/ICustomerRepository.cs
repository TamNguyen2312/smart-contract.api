using App.Entity.DTOs.Customer;
using App.Entity.Entities;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface ICustomerRepository
{
    Task<BaseResponse> CreateUpdateCustomer(Customer customer, long userId);
    Task<List<Customer>> GetAllCustomers(CustomerGetListDTO dto, long userId);
    Task<Customer> GetCustomer(long customerId, long userId);
    Task<BaseResponse> DeleteCustomer(long customerId, long userId);
}