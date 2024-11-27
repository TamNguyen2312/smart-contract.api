using App.Entity.DTOs.ContractAppendix;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface IContractAppendixRepository
{
    Task<BaseResponse> CreateUpdateContractAppendix(ContractAppendix contractAppendix, ApplicationUser user);
    Task<List<ContractAppendix>> GetContractAppendicesByContract(ContractAppendixGetListDTO dto, long contractId);
    Task<ContractAppendix> GetContractAppendix(long contractId, long contractAppendixId);
}