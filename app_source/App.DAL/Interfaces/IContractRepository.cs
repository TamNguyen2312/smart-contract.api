
using App.Entity.DTOs.Contract;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface IContractRepository
{
    Task<BaseResponse> CreateContract(Entity.Entities.Contract contract, ApplicationUser user, Employee employee);
    Task<BaseResponse> UpdateContract(Contract contract, ApplicationUser user);
    Task<List<Contract>> GetContractsByManager(ContractGetListDTO dto, string managerId);
    Task<List<Contract>> GetContractsByEmployee(ContractGetListDTO dto, string employeeId);
    Task<Contract> GetContract(long id);
    Task<bool> HasEmployeeAccessToContract(string employeeId, long contractId);
    Task<bool> HasManagerAccessToContract(string managerId, long contractId);
    Task<List<Contract>> GetContractsByAdmin(ContractGetListDTO dto);
    Task<List<long>> IsContractManagedByWhichDepartment(long contractId);

    Task<BaseResponse> CreateUpdateContractDepartmentAssign(ContractDepartmentAssign contractDepartmentAssign,
        ApplicationUser user);

    Task<BaseResponse> CreateUpdateEmpContract(EmpContract empContract, ApplicationUser user);
}
