using App.Entity.DTOs.Manager;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;

namespace App.DAL.Interfaces;

public interface IManagerRepository
{
    Task<BaseResponse> CreateUpdateManager(Manager manager, ApplicationUser user);
    Task<Manager> GetManager(long userId);
    Task<Manager> GetManagerByDepartmentId(long departmentId);
    Task<List<UserManagerDTO>> GetAllManager(AccountGetListDTO dto);
    Task<bool> HasManagerInDepartment(long departmentId);
}