using App.Entity.DTOs.Customer;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface ICustomerBizLogic
{
    Task<BaseResponse> CreateUpdateCustomer(CustomerRequestDTO dto, long userId);
}