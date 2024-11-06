using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.Customer;
using FS.Commons.Models;

namespace App.BLL.Implements;

public class CustomerBizLogic : ICustomerBizLogic
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerBizLogic(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    public async Task<BaseResponse> CreateUpdateCustomer(CustomerRequestDTO dto, long userId)
    {
        var enity = dto.GetEntity();
        var response = await _customerRepository.CreateUpdateCustomer(enity, userId);
        return response;
    }
}