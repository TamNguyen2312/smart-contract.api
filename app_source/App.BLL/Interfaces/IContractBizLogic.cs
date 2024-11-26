using System;
using App.Entity.DTOs.Contract;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface IContractBizLogic
{
    Task<BaseResponse> CreateContract(ContractRequestDTO dto, long userId);
    Task<BaseResponse> UpdateContract(ContractUpdateDTO dto, long userId);
    Task<List<ContractViewDTO>> GetContractsByManager(ContractGetListDTO dto, string managerId);
    Task<List<ContractViewDTO>> GetContractsByEmployee(ContractGetListDTO dto, string employeeId);
    Task<ContractViewDTO> GetContract(long id);
    Task<bool> HasEmployeeAccessToContract(string employeeId, long contractId);
    Task<bool> HasManagerAccessToContract(string managerId, long contractId);
    Task<List<ContractViewDTO>> GetContractsByAdmin(ContractGetListDTO dto);
    Task<BaseResponse> AssignContractToDepartment(ContractAssignRequestDTO dto, long userId);
    Task<BaseResponse> UpdateContractDepartmentAssign(ContractAssignUpdateDTO dto, long userId);
    Task<BaseResponse> AssignContractToEmployee(EmpContractRequestDTO dto, long userId);

    Task<List<ContractDepartmentAssignViewDTO>> GetContractDepartmentAssignsByManager(
        ContractDepartmentAssignGetListDTO dto, string managerId);

    Task<List<ContractDepartmentAssignViewDTO>> GetContractDepartmentAssignsByAdmin(
        ContractDepartmentAssignGetListDTO dto);

    Task<bool> HasManagerAccessToContractDepartmetnAssign(long contractId, long departmentId, string managerId);
    Task<ContractDepartmentAssignViewDTO> GetContractDepartmentAssign(long contractId, long departmentId);

    Task<List<EmpContractViewDTO>> GetEmpContractsByEmployee(EmpContractGetListDTO dto, string employeeId);
    Task<List<EmpContractViewDTO>> GetEmpContractsByManager(EmpContractGetListDTO dto, string managerId);
    Task<EmpContractViewDTO> GetEmpContract(string employeeId, long contractId);
    Task<bool> HasEmployeeAccessToEmpContract(string employeeId, long contractId, string loggedEmp);
    Task<bool> HasManagerAccessToEmpContract(string employeeId, long contractId, string managerId);
}
