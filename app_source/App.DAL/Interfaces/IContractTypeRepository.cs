using System;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface IContractTypeRepository
{
    Task<BaseResponse> CreateUpdateContractType(ContractType contractType, ApplicationUser user);
    Task<ContractType> GetContractTypeById(long id);
}
