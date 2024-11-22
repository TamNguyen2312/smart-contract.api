using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.Customer;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class CustomerBizLogic : ICustomerBizLogic
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IIdentityRepository _identityRepository;

    public CustomerBizLogic(ICustomerRepository customerRepository, IIdentityRepository identityRepository)
    {
        _customerRepository = customerRepository;
        _identityRepository = identityRepository;
    }
    public async Task<BaseResponse> CreateUpdateCustomer(CustomerRequestDto dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var userRoles = await _identityRepository.GetRolesAsync(userId);
        var response = await _customerRepository.CreateUpdateCustomer(entity, user);
        return response;
    }

    public async Task<List<CustomerViewDTO>> GetAllCustomers(CustomerGetListDTO dto, long userId)
    {
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _customerRepository.GetAllCustomers(dto, user);
        return await GetCustomerViews(response);
    }

    /// <summary>
    /// This is used to get customers that manager has access
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="managerId"></param>
    /// <returns></returns>
    public async Task<List<CustomerViewDTO>> GetCustomersByManagerAsync(CustomerGetListDTO dto, string managerId)
    {
        var data = await _customerRepository.GetCustomersByManagerAsync(dto, managerId);
        var response = await GetCustomerViews(data);
        return response;
    }

    /// <summary>
    /// check permission of manager when access Customer information
    /// </summary>
    /// <param name="managerId"></param>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public async Task<bool> ManagerHasAccessToCustomerAsync(string managerId, long customerId)
    {
        var response = await _customerRepository.ManagerHasAccessToCustomerAsync(managerId, customerId);
        return response;
    }

    public async Task<CustomerViewDTO> GetCustomer(long customerId, long userId)
    {
        var user = await _identityRepository.GetByIdAsync(userId);
        var data = await _customerRepository.GetCustomer(customerId, user);
        if (data == null) return null;
        var response = await GetCustomerView(data);
        return response;
    }

    public async Task<CustomerViewDTO> GetCustomer(long customerId)
    {
        var data = await _customerRepository.GetCustomer(customerId);
        if (data == null) return null;
        var response = await GetCustomerView(data);
        return response;
    }


    public async Task<CustomerViewDTO> GetCustomerByEmail(string email)
    {
        var data = await _customerRepository.GetCustomerByEmail(email);
        if (data == null) return null;
        var response = await GetCustomerView(data);
        return response;
    }

    public async Task<BaseResponse> DeleteCustomer(long customerId, long userId)
    {
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _customerRepository.DeleteCustomer(customerId, user);
        return response;
    }

    #region PRIVATE

    /// <summary>
    /// This is used to convert an user to userView
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<UserViewDTO> GetUserView(long userId)
    {
        var user = await _identityRepository.GetByIdAsync(userId);
        var userRoles = await _identityRepository.GetRolesAsync(userId);
        if (user == null) return null;
        var userView = new UserViewDTO(user, userRoles.ToList());
        return userView;
    }

    
    /// <summary>
    /// This is used to convert a customer to customer view
    /// </summary>
    /// <param name="customer"></param>
    /// <returns></returns>
    private async Task<CustomerViewDTO> GetCustomerView(Customer customer)
    {
        var customerView = new CustomerViewDTO(customer);
        return customerView;
    }

    
    /// <summary>
    /// This is used to convert a collection of customer to a collection of customer views
    /// </summary>
    /// <param name="customers"></param>
    /// <returns></returns>
    private async Task<List<CustomerViewDTO>> GetCustomerViews(List<Customer> customers)
    {
        var customerViews = new List<CustomerViewDTO>();
        foreach (var customer in customers)
        {
            var customerView = await GetCustomerView(customer);
            customerViews.Add(customerView);
        }

        return customerViews;
    }

    #endregion
}