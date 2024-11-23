using System;
using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.Contract;
using App.Entity.Entities;
using FS.Commons.Models;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class ContractBizLogic : IContractBizLogic
{
    private readonly IContractRepository _contractRepository;
    private readonly IIdentityRepository _identityRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IContractTypeRepository _contractTypeRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICustomerDepartmentAssignRepository _customerDepartmentAssignRepository;

    public ContractBizLogic(IContractRepository contractRepository,
        IIdentityRepository identityRepository,
        ICustomerRepository customerRepository,
        IContractTypeRepository contractTypeRepository,
        IEmployeeRepository employeeRepository,
        ICustomerDepartmentAssignRepository customerDepartmentAssignRepository)
    {
        _contractRepository = contractRepository;
        _identityRepository = identityRepository;
        _customerRepository = customerRepository;
        _contractTypeRepository = contractTypeRepository;
        _employeeRepository = employeeRepository;
        _customerDepartmentAssignRepository = customerDepartmentAssignRepository;
    }

    public async Task<BaseResponse> CreateContract(ContractRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var employee = await _employeeRepository.GetEmployee(userId);
        
        var isAssigned =
            await _customerDepartmentAssignRepository.IsCustomerAssignedIn(dto.CustomerId, employee.DepartmentId);
        if (!isAssigned)
            return new BaseResponse
                { IsSuccess = false, Message = "Khách hàng không thuộc quyền quản lý của phòng ban" };
        
        var response = await _contractRepository.CreateContract(entity, user, employee);
        return response;
    }
    
    public async Task<BaseResponse> UpdateContract(ContractUpdateDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _contractRepository.UpdateContract(entity, user);
        return response;
    }

    public async Task<List<ContractViewDTO>> GetContractsByManager(ContractGetListDTO dto, string managerId)
    {
        var data = await _contractRepository.GetContractsByManager(dto, managerId);
        var response = await GetContractViews(data);
        return response;
    }

    public async Task<List<ContractViewDTO>> GetContractsByEmployee(ContractGetListDTO dto, string employeeId)
    {
        var data = await _contractRepository.GetContractsByEmployee(dto, employeeId);
        var response = await GetContractViews(data);
        return response;
    }

    public async Task<ContractViewDTO> GetContract(long id)
    {
        var data = await _contractRepository.GetContract(id);
        if (data == null) return null;
        var response = await GetContractView(data);
        return response;
    }

    public async Task<bool> HasManagerAccessToContract(string managerId, long contractId)
    {
        return await _contractRepository.HasManagerAccessToContract(managerId, contractId);
    }

    public async Task<bool> HasEmployeeAccessToContract(string employeeId, long contractId)
    {
        return await _contractRepository.HasEmployeeAccessToContract(employeeId, contractId);
    }


    public async Task<List<ContractViewDTO>> GetContractsByAdmin(ContractGetListDTO dto)
    {
        var data = await _contractRepository.GetContractsByAdmin(dto);
        var response = await GetContractViews(data);
        return response;
    }

    #region PRIVATE

    private async Task<ContractViewDTO> GetContractView(Contract contract)
    {
        var customer = await _customerRepository.GetCustomer(contract.CustomerId);
        if (customer == null) return null;

        var contractType = await _contractTypeRepository.GetContractTypeById(contract.ContractTypeId);
        if (contractType == null) return null;

        var view = new ContractViewDTO(contract, customer, contractType);
        return view;
    }

    private async Task<List<ContractViewDTO>> GetContractViews(List<Contract> contracts)
    {
        var views = new List<ContractViewDTO>();
        foreach (var contract in contracts)
        {
            var view = await GetContractView(contract);
            views.Add(view);
        }

        return views;
    }

    #endregion
}
