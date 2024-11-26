using App.Entity.DTOs.ContractTerm;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface IContractTermBizLogic
{
    Task<BaseResponse> CreateUpdateContractTerm(ContractTermRequestDTO dto, long userId);
}