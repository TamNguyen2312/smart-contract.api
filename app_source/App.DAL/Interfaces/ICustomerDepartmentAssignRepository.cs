using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface ICustomerDepartmentAssignRepository
{
    Task<BaseResponse> CreateUpdateCusomterDepartmentAssign(CustomerDepartmentAssign assign, ApplicationUser user);
}