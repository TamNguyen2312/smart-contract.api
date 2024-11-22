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

    /// <summary>
    /// This is used to get personal manager profile
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
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

    public async Task<ManagerViewDTO> GetManagerByDepartmentId(long departmentId)
    {
        var manager = await _managerRepository.GetManager(departmentId);
        if (manager == null) return null;
        var managerView = await GetManagerView(manager);
        return managerView;
    }

    public async Task<List<ManagerViewDTO>> GetAllManager(AccountGetListDTO dto)
    {
        var data = await _managerRepository.GetAllManager(dto);
        var response = await GetManagerViews(data);
        return response;
    }

    /// <summary>
    /// Check that department has manager
    /// </summary>
    /// <param name="departmentId"></param>
    /// <returns></returns>
    public async Task<bool> HasManagerInDepartment(long departmentId)
    {
        return await _managerRepository.HasManagerInDepartment(departmentId);
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
        var department = await _departmentRepository.GetDepartment(manager.DepartmentId);
        var view = new ManagerViewDTO(user, userRoles.ToList(), manager, department);
        return view;
    }

    private async Task<ManagerViewDTO> GetManagerView(Manager manager, ApplicationUser user, List<string> userRoles)
    {
        var department = await _departmentRepository.GetDepartment(manager.DepartmentId);
        var view = new ManagerViewDTO(user, userRoles, manager, department);
        return view;
    }

    private async Task<ManagerViewDTO> GetManagerView(Manager manager, ApplicationUser user)
    {
        var department = await _departmentRepository.GetDepartment(manager.DepartmentId);
        var userRoles = await _identityRepository.GetRolesAsync(user.Id);
        var view = new ManagerViewDTO(user, userRoles.ToList(), manager, department);
        return view;
    }

    private async Task<List<ManagerViewDTO>> GetManagerViews(List<UserManagerDTO> userManagers)
    {
        var response = new List<ManagerViewDTO>();
        foreach (var item in userManagers)
        {
            var managerView = await GetManagerView(item.Manager, item.User);
            response.Add(managerView);
        }

        return response;
    }
    
    #endregion
}