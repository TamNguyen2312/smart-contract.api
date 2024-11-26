using App.Entity.DTOs.ContractDocument;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface IContractDocumentBizLogic
{
    Task<BaseResponse> CreateUpdateContractDocument(ContractDocumentRequestDTO dto, long userId);
    Task<List<ContractDocumentViewDTO>> GetContractDocumentsByContract(ContractDocumentGetListDTO dto, long contractId);
    Task<ContractDocumentViewDTO> GetContractDocument(long contractId, long contractDocumentId);
}