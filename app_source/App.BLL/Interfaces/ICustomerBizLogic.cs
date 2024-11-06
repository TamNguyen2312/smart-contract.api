using App.Entity.DTOs.Customer;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface ICustomerBizLogic
{
    Task<BaseResponse> CreateUpdateCustomer(CustomerRequestDTO dto, long userId);
    Task<List<CustomerViewDTO>> GetAllCustomers(CustomerGetListDTO dto, long userId);
    Task<CustomerViewDTO> GetCustomer(long customerId, long userId);
}