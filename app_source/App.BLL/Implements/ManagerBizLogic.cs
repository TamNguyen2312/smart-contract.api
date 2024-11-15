using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.Employee;
using App.Entity.DTOs.Manager;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class ManagerBizLogic : IManagerBizLogic
{
    private readonly IManagerRepository _managerRepository;
    private readonly IIdentityRepository _identityRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public ManagerBizLogic(IManagerRepository managerRepository, IIdentityRepository identityRepository,
        IDepartmentRepository departmentRepository)
    {
        _managerRepository = managerRepository;
        _identityRepository = identityRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<BaseResponse> CreateUpdateManager(ManagerRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        if (user == null) return new BaseResponse { IsSuccess = false, Message = Constants.EXPIRED_SESSION };
        var response = await _managerRepository.CreateUpdateManager(entity, user);
        return response;
    }

    public async Task<ManagerViewDTO> GetManager(long userId)
    {
        var manager = await _managerRepository.GetManager(userId);
        if (manager == null) return null;
        var managerView = await GetManagerView(manager);
        return managerView;
    }

    public async Task<ManagerViewDTO> GetManager(ApplicationUser user, List<string> userRoles)
    {
        var manager = await _managerRepository.GetManager(user.Id);
        if (manager == null) return null;
        var managerView = await GetManagerView(manager, user, userRoles);
        return managerView;
    }

    #region CONVERT

    /// <summary>
    /// This is used to convert a manager to a manager view
    /// </summary>
    /// <param name="manager"></param>
    /// <returns></returns>
    private async Task<ManagerViewDTO> GetManagerView(Manager manager)
    {
        var userId = Convert.ToInt64(manager.Id);
        var user = await _identityRepository.GetByIdAsync(userId);
        var userRoles = await _identityRepository.GetRolesAsync(userId);
        var department = await _departmentRepository.GetDepartment(manager.DepartmentId, userId);
        var view = new ManagerViewDTO(user, userRoles.ToList(), manager, department);
        return view;
    }

    private async Task<ManagerViewDTO> GetManagerView(Manager manager, ApplicationUser user, List<string> userRoles)
    {
        var department = await _departmentRepository.GetDepartment(manager.DepartmentId, user.Id);
        var view = new ManagerViewDTO(user, userRoles, manager, department);
        return view;
    }

    #endregion
}