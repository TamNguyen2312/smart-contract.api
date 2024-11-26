using System;
using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.Contract;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
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
    private readonly IDepartmentRepository _departmentRepository;

    public ContractBizLogic(IContractRepository contractRepository,
        IIdentityRepository identityRepository,
        ICustomerRepository customerRepository,
        IContractTypeRepository contractTypeRepository,
        IEmployeeRepository employeeRepository,
        ICustomerDepartmentAssignRepository customerDepartmentAssignRepository,
        IDepartmentRepository departmentRepository)
    {
        _contractRepository = contractRepository;
        _identityRepository = identityRepository;
        _customerRepository = customerRepository;
        _contractTypeRepository = contractTypeRepository;
        _employeeRepository = employeeRepository;
        _customerDepartmentAssignRepository = customerDepartmentAssignRepository;
        _departmentRepository = departmentRepository;
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
        var departmentIds = await _contractRepository.IsContractManagedByWhichDepartment(dto.Id);
        var flag = 0;
        foreach (var departmentId in departmentIds)
        {
            var isAssigned =
                await _customerDepartmentAssignRepository.IsCustomerAssignedIn(dto.CustomerId, departmentId);
            if (isAssigned) flag = 1;
            else continue;
        }

        if (flag != 1)
        {
            return new BaseResponse
                { IsSuccess = false, Message = "Khách hàng không thuộc quyền quản lý của phòng ban" };
        }
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _contractRepository.UpdateContract(entity, user);
        return response;
    }

    public async Task<BaseResponse> AssignContractToDepartment(ContractAssignRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _contractRepository.CreateUpdateContractDepartmentAssign(entity, user);
        return response;
    }

    public async Task<BaseResponse> UpdateContractDepartmentAssign(ContractAssignUpdateDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _contractRepository.CreateUpdateContractDepartmentAssign(entity, user);
        return response;
    }

    public async Task<List<ContractDepartmentAssignViewDTO>> GetContractDepartmentAssignsByManager(
        ContractDepartmentAssignGetListDTO dto, string managerId)
    {
        var data = await _contractRepository.GetContractDepartmentAssignsByManager(dto, managerId);
        var response = await GetContractDeparmentAssignViews(data);
        return response;
    }

    public async Task<List<ContractDepartmentAssignViewDTO>> GetContractDepartmentAssignsByAdmin(
        ContractDepartmentAssignGetListDTO dto)
    {
        var data = await _contractRepository.GetContractDepartmentAssignsByAdmin(dto);
        var response = await GetContractDeparmentAssignViews(data);
        return response;
    }

    public async Task<bool> HasManagerAccessToContractDepartmetnAssign(long contractId, long departmentId, string managerId)
    {
        return await _contractRepository.HasManagerAccessToContractDepartmetnAssign(contractId, departmentId,
            managerId);
    }
    
    public async Task<ContractDepartmentAssignViewDTO> GetContractDepartmentAssign(long contractId, long departmentId)
    {
        var data = await _contractRepository.GetContractDepartmentAssign(contractId, departmentId);
        if (data == null) return null;
        var response = await GetContractDeparmentAssignView(data);
        return response;
    }

    public async Task<BaseResponse> AssignContractToEmployee(EmpContractRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _contractRepository.CreateUpdateEmpContract(entity, user);
        return response;
    }

    public async Task<List<EmpContractViewDTO>> GetEmpContractsByEmployee(EmpContractGetListDTO dto, string employeeId)
    {
        var data = await _contractRepository.GetEmpContractsByEmployee(dto, employeeId);
        var response = GetEmpContractViews(data);
        return response;
    }
    
    public async Task<List<EmpContractViewDTO>> GetEmpContractsByManager(EmpContractGetListDTO dto, string managerId)
    {
        var data = await _contractRepository.GetEmpContractsByManager(dto, managerId);
        var response = GetEmpContractViews(data);
        return response;
    }
    

    public async Task<EmpContractViewDTO> GetEmpContract(string employeeId, long contractId)
    {
        var data = await _contractRepository.GetEmpContract(employeeId, contractId);
        if (data == null) return null;
        var response = GetEmpContractView(data);
        return response;
    }

    public async Task<bool> HasEmployeeAccessToEmpContract(string employeeId, long contractId, string loggedEmp)
    {
        return await _contractRepository.HasEmployeeAccessToEmpContract(employeeId, contractId, loggedEmp);
    }

    public async Task<bool> HasManagerAccessToEmpContract(string employeeId, long contractId, string managerId)
    {
        return await _contractRepository.HasManagerAccessToEmpContract(employeeId, contractId, managerId);
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

    private async Task<ContractDepartmentAssignViewDTO> GetContractDeparmentAssignView(ContractDepartmentAssign contractDepartmentAssign)
    {
        var department = await _departmentRepository.GetDepartment(contractDepartmentAssign.DepartmentId);
        if (department == null) return null;
        var view = new ContractDepartmentAssignViewDTO(contractDepartmentAssign, department);
        return view;
    }
    
    private async Task<List<ContractDepartmentAssignViewDTO>> GetContractDeparmentAssignViews(List<ContractDepartmentAssign> contractDepartmentAssigns)
    {
        var response = new List<ContractDepartmentAssignViewDTO>();
        foreach (var item in contractDepartmentAssigns)
        {
            var view = await GetContractDeparmentAssignView(item);
            response.Add(view);
        }

        return response;
    }
    
    private EmpContractViewDTO GetEmpContractView(EmpContract empContract)
    {
        var view = new EmpContractViewDTO(empContract);
        return view;
    }

    private List<EmpContractViewDTO> GetEmpContractViews(List<EmpContract> empContracts)
    {
        return empContracts.Select(x => new EmpContractViewDTO(x)).ToList();
    }

    #endregion
}
