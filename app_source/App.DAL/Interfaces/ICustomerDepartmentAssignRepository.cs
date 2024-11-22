using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface ICustomerDepartmentAssignRepository
{
    Task<BaseResponse> CreateUpdateCusomterDepartmentAssign(CustomerDepartmentAssign assign, ApplicationUser user);
    Task<CustomerDepartmentAssign> GetCustomerDepartmentAssign(long id);
    Task<CustomerDepartmentAssign> GetCustomerDepartmentAssign(long customerId, long departmentId);
}