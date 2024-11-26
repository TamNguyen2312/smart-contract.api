using App.Entity.DTOs.ContractTerm;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface IContractTermBizLogic
{
    Task<BaseResponse> CreateUpdateContractTerm(ContractTermRequestDTO dto, long userId);
    Task<List<ContractTermViewDTO>> GetContractTermsByContract(ContractTermGetListDTO dto, long contractId);
    Task<ContractTermViewDTO> GetContractTerm(long contractId, long contractTermId);
}