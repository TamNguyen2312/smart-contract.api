using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.Employee;
using App.Entity.Entities;
using FS.Commons.Models;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class EmployeeBizLogic : IEmployeeBizLogic
{
    private readonly IEmployeeRepository _repository;
    private readonly IIdentityRepository _identityRepository;

    public EmployeeBizLogic(IEmployeeRepository repository, IIdentityRepository identityRepository)
    {
        _repository = repository;
        _identityRepository = identityRepository;
    }

    public async Task<BaseResponse> CreateUpdateEmployee(EmployeeRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var response = await _repository.CreateUpdateEmployee(entity, userId);
        return response;
    }

    public async Task<EmployeeViewDTO> GetEmployee(long userId)
    {
        var emp = await _repository.GetEmployee(userId);
        if (emp == null) return null;
        var empView = await GetEmpView(emp);
        if (empView == null) return null;
        return empView;
    }

    #region PRIVATE

    private async Task<EmployeeViewDTO> GetEmpView(Employee emp)
    {
        var user = await _identityRepository.GetByIdAsync(emp.Id);
        if (user == null) return null;
        var userRoles = await _identityRepository.GetRolesAsync(emp.Id);
        var empView = new EmployeeViewDTO(user, userRoles.ToList(), emp);
        return empView;
    }

    #endregion
}