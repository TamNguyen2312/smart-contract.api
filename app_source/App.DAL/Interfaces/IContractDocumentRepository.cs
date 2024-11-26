using App.Entity.DTOs.ContractDocument;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface IContractDocumentRepository
{
    Task<BaseResponse> CreateUpdateContractDocument(ContractDocument contractDocument, ApplicationUser user);
    Task<List<ContractDocument>> GetContractDocumentsByContract(ContractDocumentGetListDTO dto, long contractId);
}