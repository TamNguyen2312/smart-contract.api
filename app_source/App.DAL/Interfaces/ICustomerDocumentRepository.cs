using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface ICustomerDocumentRepository
{
    Task<BaseResponse> CreateUpdateCustomerDocument(CustomerDocument customerDocument, ApplicationUser user);
}