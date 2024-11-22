using App.Entity.DTOs.CustomerDocument;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface ICustomerDocumentRepository
{
    Task<BaseResponse> CreateUpdateCustomerDocument(CustomerDocument customerDocument, ApplicationUser user);
    Task<CustomerDocument> GetCustomerDocument(long id);
    Task<List<CustomerDocument>> GetAllCustomerDocuments(CustomerDocumentGetListDTO dto, string userName);

    Task<List<CustomerDocument>> GetCustomerDocumentsByManagerAsync(CustomerDocumentGetListDTO dto,
        string managerId);

    Task<bool> ManagerHasAccessToCustomerDocumentAsync(string managerId, long customerDocumentId);
}