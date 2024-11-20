using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface IManagerRepository
{
    Task<BaseResponse> CreateUpdateManager(Manager manager, ApplicationUser user);
    Task<Manager> GetManager(long userId);
    Task<Manager> GetManagerByDepartmentId(long departmentId);
}