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
    Task<BaseResponse> UpdateContractAssign(ContractAssignUpdateDTO dto, long userId);
}
