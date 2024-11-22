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

    public async Task<BaseResponse> CreateUpdateCustomerDepartmentAssgin(CustomerDepartmentAssginRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _customerDepartmentAssignRepository.CreateUpdateCusomterDepartmentAssign(entity, user);
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

    #endregion
}