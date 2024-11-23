
using App.Entity.DTOs.Contract;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface IContractRepository
{
    Task<BaseResponse> CreateContract(Entity.Entities.Contract contract, ApplicationUser user, Employee employee);
    Task<List<Contract>> GetContractsByManager(ContractGetListDTO dto, string managerId);
}
