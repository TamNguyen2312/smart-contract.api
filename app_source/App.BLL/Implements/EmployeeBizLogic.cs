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

public class EmployeeBizLogic : IEmployeeBizLogic
{
    private readonly IEmployeeRepository _repository;
    private readonly IIdentityRepository _identityRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public EmployeeBizLogic(IEmployeeRepository repository, IIdentityRepository identityRepository,
        IDepartmentRepository departmentRepository)
    {
        _repository = repository;
        _identityRepository = identityRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<BaseResponse> CreateUpdateEmployee(EmployeeRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        if (user == null) return new BaseResponse { IsSuccess = false, Message = Constants.EXPIRED_SESSION };
        var response = await _repository.CreateUpdateEmployee(entity, user);
        return response;
    }

    /// <summary>
    /// Get an employee profile also check permission if the logged user is manager
    /// </summary>
    /// <param name="user"></param>
    /// <param name="userRoles"></param>
    /// <param name="loggedManager"></param>
    /// <returns></returns>
    public async Task<EmployeeViewDTO> GetEmployee(ApplicationUser user, List<string> userRoles, ManagerViewDTO loggedManager)
    {
        var emp = await _repository.GetEmployee(user.Id);
        if (emp == null || emp.DepartmentId != loggedManager.DepartmentId) return null;
        var empView = await GetEmpView(emp, user, userRoles);
        return empView;
    }
    
    /// <summary>
    /// This is used to get personal emp profile
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<EmployeeViewDTO> GetEmployee(long userId)
    {
        var emp = await _repository.GetEmployee(userId);
        if (emp == null) return null;
        var empView = await GetEmpView(emp);
        return empView;
    }

    public async Task<EmployeeViewDTO> GetEmployee(ApplicationUser user, List<string> userRoles)
    {
        var emp = await _repository.GetEmployee(user.Id);
        if (emp == null) return null;
        var empView = await GetEmpView(emp, user, userRoles);
        return empView;
    }

    public async Task<List<EmployeeViewDTO>> GetAllEmployee(AccountGetListDTO dto)
    {
        var data = await _repository.GetAllEmployee(dto);
        var response = await GetEmpViews(data);
        return response;
    }

    #region PRIVATE

    private async Task<EmployeeViewDTO> GetEmpView(Employee emp)
    {
        var userId = Convert.ToInt64(emp.Id);
        var user = await _identityRepository.GetByIdAsync(userId);
        var userRoles = await _identityRepository.GetRolesAsync(userId);
        var department = await _departmentRepository.GetDepartment(emp.DepartmentId, userId);
        var empView = new EmployeeViewDTO(user, userRoles.ToList(), emp, department);
        return empView;
    }

    private async Task<EmployeeViewDTO> GetEmpView(Employee emp, ApplicationUser user, List<string> userRoles)
    {
        var department = await _departmentRepository.GetDepartment(emp.DepartmentId, user.Id);
        var empView = new EmployeeViewDTO(user, userRoles.ToList(), emp, department);
        return empView;
    }
    
    private async Task<EmployeeViewDTO> GetEmpView(Employee emp, ApplicationUser user)
    {
        var department = await _departmentRepository.GetDepartment(emp.DepartmentId, user.Id);
        var userRoles = await _identityRepository.GetRolesAsync(user.Id);
        var empView = new EmployeeViewDTO(user, userRoles.ToList(), emp, department);
        return empView;
    }

    private async Task<List<EmployeeViewDTO>> GetEmpViews(List<UserEmpDTO> userEmps)
    {
        var response = new List<EmployeeViewDTO>();
        foreach (var item in userEmps)
        {
            var empView = await GetEmpView(item.Employee, item.User);
            response.Add(empView);
        }

        return response;
    }

    #endregion
}