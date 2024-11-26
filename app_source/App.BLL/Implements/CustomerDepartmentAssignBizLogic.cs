using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.CustomerDeparmentAssign;
using App.Entity.Entities;
using FS.Commons.Models;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class CustomerDepartmentAssignBizLogic : ICustomerDepartmentAssignBizLogic
{
    private readonly ICustomerDepartmentAssignRepository _customerDepartmentAssignRepository;
    private readonly IIdentityRepository _identityRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public CustomerDepartmentAssignBizLogic(ICustomerDepartmentAssignRepository customerDepartmentAssignRepository
                                            , IIdentityRepository identityRepository
                                            , ICustomerRepository customerRepository
                                            , IDepartmentRepository departmentRepository)
    {
        _customerDepartmentAssignRepository = customerDepartmentAssignRepository;
        _identityRepository = identityRepository;
        _customerRepository = customerRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<BaseResponse> CreateUpdateCustomerDepartmentAssgin(CustomerDepartmentAssignRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _customerDepartmentAssignRepository.CreateUpdateCusomterDepartmentAssign(entity, user);
        return response;
    }

    public async Task<List<CustomerDepartmentAssignViewDTO>> GetCustomerDepartmentAssignsByAdmin(CustomerDepartmentAssignGetListDTO dto,
        string userName)
    {
        var data = await _customerDepartmentAssignRepository.GetCustomerDepartmentAssignsByAdmin(dto, userName);
        var response = await GetCustomerDepartmentAssignViews(data);
        return response;
    }

    public async Task<List<CustomerDepartmentAssignViewDTO>> GetCustomerDepartmentAssignsByManager(
        CustomerDepartmentAssignGetListDTO dto, string managerId)
    {
        var data = await _customerDepartmentAssignRepository.GetCustomerDepartmentAssignsByManager(dto, managerId);
        var response = await GetCustomerDepartmentAssignViews(data);
        return response;
    }

    public async Task<bool> ManagerHasAccessToAssignAsync(string managerId, long id)
    {
        var response = await _customerDepartmentAssignRepository.ManagerHasAccessToAssignAsync(managerId, id);
        return response;
    }

    /// <summary>
    /// This is used to check whether customer is assigned in department
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="departmentId"></param>
    /// <returns></returns>
    public async Task<bool> IsCustomerAssignedIn(long customerId, long departmentId)
    {
        var response = await _customerDepartmentAssignRepository.IsCustomerAssignedIn(customerId, departmentId);
        return response;
    }
    

    public async Task<CustomerDepartmentAssignViewDTO> GetCustomerDepartmentAssign(long id)
    {
        var data = await _customerDepartmentAssignRepository.GetCustomerDepartmentAssign(id);
        if (data == null) return null;
        var response = await GetCustomerDepartmentAssignView(data);
        return response;
    }

    // public async Task<CustomerDepartmentAssign> GetCustomerDepartmentAssign(long customerId, long departmentId)
    // {
    //     var data = await _customerDepartmentAssignRepository.GetCustomerDepartmentAssign(customerId, departmentId);
    // }

    #region PRIVATE

    private async Task<CustomerDepartmentAssignViewDTO> GetCustomerDepartmentAssignView(CustomerDepartmentAssign customerDepartmentAssign)
    {
        var customer = await _customerRepository.GetCustomer(customerDepartmentAssign.CustomerId);
        if (customer == null) return null;

        var department = await _departmentRepository.GetDepartment(customerDepartmentAssign.DeparmentId);
        if (department == null) return null;

        var view = new CustomerDepartmentAssignViewDTO(customerDepartmentAssign, customer, department);
        return view;
    }

    private async Task<List<CustomerDepartmentAssignViewDTO>> GetCustomerDepartmentAssignViews(
        List<CustomerDepartmentAssign> customerDepartmentAssigns)
    {
        var response = new List<CustomerDepartmentAssignViewDTO>();
        foreach (var item in customerDepartmentAssigns)
        {
            var view = await GetCustomerDepartmentAssignView(item);
            response.Add(view);
        }

        return response;
    }

    #endregion
}