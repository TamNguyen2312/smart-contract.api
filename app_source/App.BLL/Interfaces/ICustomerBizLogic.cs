using App.Entity.DTOs.Customer;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface ICustomerBizLogic
{
    Task<BaseResponse> CreateUpdateCustomer(CustomerRequestDto dto, long userId);
    Task<List<CustomerViewDTO>> GetAllCustomers(CustomerGetListDTO dto, long userId);
    Task<List<CustomerViewDTO>> GetCustomersByManagerAsync(CustomerGetListDTO dto, string managerId);
    Task<bool> ManagerHasAccessToCustomerAsync(string managerId, long customerId);
    Task<List<CustomerViewDTO>> GetDropdownCustomersByManagerOrEmployee(string id, bool isManager);
    Task<List<CustomerViewDTO>> GetDropdownCustomersByAdmin(long departmentId);
    Task<CustomerViewDTO> GetCustomer(long customerId, long userId);
    Task<CustomerViewDTO> GetCustomer(long customerId);
    Task<CustomerViewDTO> GetCustomerByEmail(string email);
    Task<BaseResponse> DeleteCustomer(long customerId, long userId);
}