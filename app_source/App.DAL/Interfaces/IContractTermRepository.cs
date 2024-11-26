using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface IContractTermRepository
{
    Task<BaseResponse> CreateUpdateContractTerm(ContractTerm contractTerm, ApplicationUser user);
}