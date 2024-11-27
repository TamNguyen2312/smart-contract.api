using App.Entity.DTOs.ContractAppendix;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface IContractAppendixBizLogic
{
    Task<BaseResponse> CreateUpdateContractAppendix(ContractAppendixRequestDTO dto, long userId);

    Task<List<ContractAppendixViewDTO>>
        GetContractAppendicesByContract(ContractAppendixGetListDTO dto, long contractId);
}