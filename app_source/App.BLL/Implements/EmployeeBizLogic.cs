using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.Employee;
using App.Entity.Entities;
using FS.Commons;
using FS.Commons.Models;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class EmployeeBizLogic : IEmployeeBizLogic
{
    private readonly IEmployeeRepository _repository;
    private readonly IIdentityRepository _identityRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public EmployeeBizLogic(IEmployeeRepository repository, IIdentityRepository identityRepository, IDepartmentRepository departmentRepository)
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

    public async Task<EmployeeViewDTO> GetEmployee(long userId)
    {
        var emp = await _repository.GetEmployee(userId);
        var empView = await GetEmpView(emp);
        return empView;
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

    #endregion
}