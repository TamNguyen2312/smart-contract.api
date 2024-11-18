using System;
using System.Diagnostics.Contracts;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface IContractRepository
{
    Task<BaseResponse> CreateContract(Entity.Entities.Contract contract, ApplicationUser user);
}
